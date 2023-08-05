using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Game;
using Game.Player;
using Game.Fields;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Network
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        public InputField RoomInput, NickNameInput;

        public static NetworkManager instance;
        public PhotonView Photonview;

        public GameObject LocalPlayer;
        public int PlayerIndex { get; private set; }
        public bool isReady;
        #endregion

        #region Private Fields


        #endregion

        #region MonoBehaviour CallBacks
        void Awake()
        {
            // SoundManager 인스턴스가 이미 있는지 확인, 이 상태로 설정
            if (instance == null)
                instance = this;

            // 인스턴스가 이미 있는 경우 오브젝트 제거
            else if (instance != this)
                Destroy(gameObject);
            
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            Connect();       
        }

        // Update is called once per frame
        void Update()
        {
            UIManager.instance.SetStatus(PhotonNetwork.NetworkClientState.ToString());
        }
        #endregion

        #region Photon Callbacks
        public override void OnConnectedToMaster()
        {
            Debug.Log("서버 접속 완료");
            //PhotonNetwork.JoinLobby();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogFormat("연결 끊김, 사유 : {0}", cause);
        }

        public override void OnJoinedLobby()
        {
            PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
            Debug.Log("로비 접속 완료");
            UIManager.instance.SetNickname(PhotonNetwork.LocalPlayer.NickName);
            UIManager.instance.TogglePanel(EGameState.LOBBY);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            string hostNickname = "Anonymous";
            // 룸 리스트 콜백은 로비에 접속했을때 자동으로 호출된다.
            // 로비에서만 호출할 수 있음...
            Debug.Log($"룸 리스트 업데이트. 현재 방 갯수 : {roomList.Count}");
            if(roomList.Count != 0)
            {
                foreach(RoomInfo room in roomList)
                {
                    

                    if (room.CustomProperties.ContainsKey("HostNickname"))
                    {
                        hostNickname = room.CustomProperties["HostNickname"] as string;

                    }

                    Debug.Log($"room Name : {room.Name}, room PlayerCount : {room.PlayerCount}, room Host : {hostNickname}");
                    RoomManager.instance.AddRoom(room.PlayerCount, room.Name, hostNickname);

                }


            }
        }

        #endregion

        #region Public Methods

        //방 만들기 및 참가
        public void CreateRoom()
        {
            ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
            customProps["HostNickname"] = PhotonNetwork.LocalPlayer.NickName; // 실제 호스트의 닉네임으로 변경

            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 4,
                CustomRoomProperties = customProps,
                CustomRoomPropertiesForLobby = new string[] { "HostNickname" } // 로비에서 호스트 닉네임을 보이도록 설정
            };

            PhotonNetwork.CreateRoom(RoomInput.text, roomOptions, null);
        }

        public void JoinRoom() => PhotonNetwork.JoinRoom(RoomInput.text);
        public void JoinOrCreateRoom()
        {
            PhotonNetwork.JoinOrCreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 10 }, null);
            //UIManager.GetComponent<UIManager>().TogglePanel(EGameState.ROOM);
        }
        public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();
        public void LeaveRoom()
        {
            UIManager.instance.TogglePanel(EGameState.LOBBY);

            PhotonNetwork.LeaveRoom();

        }
        public void LeaveRoom(string RoomName)
        {
            PhotonNetwork.LeaveRoom();
        }

        public void GeneratePlayer(Vector3 position)
        {
            GameObject newPlayer;
            newPlayer = PhotonNetwork.Instantiate("Player",
                    position, Quaternion.identity);
            newPlayer.GetComponent<PlayerManager>().SetName(PhotonNetwork.LocalPlayer.NickName);
            LocalPlayer = newPlayer;
            

            //LocalPlayer = newPlayer;
        }

        public void GenerateTestPlayer()
        {
            Debug.Log("테스트 플레이어 생성!");
            PlayerManager newTestPlayer;
            newTestPlayer = PhotonNetwork.Instantiate("TestPlayer",
                    new Vector3 (0,0,0), Quaternion.identity).GetComponent<PlayerManager>();
        }

        public void Connect() => PhotonNetwork.ConnectUsingSettings();


        public void DisConnect() => PhotonNetwork.Disconnect();
        #endregion

        public void JoinLobby() => PhotonNetwork.JoinLobby();

        public void SetUser()
        {
            PlayerIndex = PhotonNetwork.PlayerList.Length - 1;
            UIManager.instance.SetUserName(PlayerIndex, PhotonNetwork.LocalPlayer.NickName);
        }



        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            ChattingManager.instance.AddChatMessage(PhotonNetwork.LocalPlayer.NickName + " 님이 방을 나가셨습니다.");
        }


        public override void OnCreatedRoom()
        {
            print("방 만들기 완료");
            RoomManager.instance.AddRoom(PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.Name, PhotonNetwork.LocalPlayer.NickName);
            //RoomManager.GetComponent<RoomManager>().AddRoom(PhotonNetwork.CurrentRoom.ToString());
        }
            
        public override void OnJoinedRoom()
        {
            UIManager.instance.SetRoomInfo(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CurrentRoom.PlayerCount);
            

            if (PhotonNetwork.IsMasterClient == true)
            {
                UIManager.instance.ToggleMaster();
            }
            UIManager.instance.TogglePanel(EGameState.ROOM);

            
            ChattingManager.instance.AddChatMessage(PhotonNetwork.LocalPlayer.NickName + " 님이 접속하셨습니다");
            SetUser();
            GenerateTestPlayer();
            
        }
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("방 만들기 실패, 사유 : {0} : {1}", returnCode, message);
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("방 참가 실패, 사유 : {0} : {1}", returnCode, message);
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            print("방 랜덤참가 실패");
        }


        public void SendChatting(string text)
        {
            //Photonview.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + text);
        }

        public void StartGame()
        {
            //MapGenerator.Instance.GenerateMap();
            Photonview.RPC("RPC_StartGame", RpcTarget.All);
        }

        [PunRPC]
        void RPC_StartGame()
        {
            Debug.Log("스타트게임");
            //DeleteObjectsWithTag("TestPlayer");
            SceneManager.LoadScene("MainScene");
        }

        public void Ready()
        {
            Debug.Log("레디");
            //실제로 포톤상 기능도 추가할 것
            if (!isReady)
            {
                UIManager.instance.GetReady(PlayerIndex);
            }
        }



        [ContextMenu("정보")]
        void Info()
        {
            if (PhotonNetwork.InRoom)
            {
                print("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
                print("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
                print("현재 바 최대인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);

                string playerStr = "방에 있는 플레이어 목록 : ";
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
                }
                print(playerStr);
            }
            else
            {
                print("접속한 인원 수  : " + PhotonNetwork.CountOfPlayers);
                print("방 개수 : " + PhotonNetwork.CountOfRooms);
                print("모든 방에 있는 인원 수  : " + PhotonNetwork.CountOfPlayersInRooms);
                print("로비에 있는지? : " + PhotonNetwork.InLobby);
                print("연결 됐는지? : " + PhotonNetwork.IsConnected);
            }
        }

        [ContextMenu("상태")]
        void Status()
        {
            Debug.Log(PhotonNetwork.NetworkClientState);
        }

        [ContextMenu("인원 체크")]
        void HeadCount()
        {
            string playerStr = "";
            print("인원 수  : " + PhotonNetwork.CurrentRoom.PlayerCount);
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                playerStr += PhotonNetwork.PlayerList[i].NickName + " : ";
            }
            print(playerStr);
        }

        private void DeleteObjectsWithTag(string tag)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject obj in objectsWithTag)
            {
                Destroy(obj);
            }
        }

    }
}