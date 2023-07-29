using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Game.Player;
using Game.Fields;

namespace Network
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        public Text StatusText;
        public InputField RoomInput, NickNameInput;
        public GameObject UIManager;
        public GameObject RoomManager;
        public static NetworkManager instance;
        public GameObject LocalPlayerPrefab;
        public PhotonView Photonview;
        public GameObject TestGround;

        public PlayerManager LocalPlayer;
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
            StatusText.text = PhotonNetwork.NetworkClientState.ToString();
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
            Debug.Log("로비 접속 완료");
            UIManager.GetComponent<UIManager>().TogglePanel(EGameState.LOBBY);


        }
        #endregion

        #region Public Methods

        //방 만들기 및 참가
        public void CreateRoom() => PhotonNetwork.CreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 10 });
        public void JoinRoom() => PhotonNetwork.JoinRoom(RoomInput.text);
        public void JoinOrCreateRoom()
        {
            PhotonNetwork.JoinOrCreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 10 }, null);
            //UIManager.GetComponent<UIManager>().TogglePanel(EGameState.ROOM);
        }
        public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();
        public void LeaveRoom()
        {
            UIManager.GetComponent<UIManager>().TogglePanel(EGameState.LOBBY);
            PhotonNetwork.LeaveRoom();

        }
        public void LeaveRoom(string RoomName)
        {
            PhotonNetwork.LeaveRoom();
        }

        public void GeneratePlayer(Vector3 position)
        {
            PlayerManager newPlayer;
            newPlayer = PhotonNetwork.Instantiate("Player",
                    position, Quaternion.identity).GetComponent<PlayerManager>();

            LocalPlayer = newPlayer;
        }

        public void GenerateTestPlayer()
        {
            Debug.Log("테스트 플레이어 생성!");
            PlayerManager newTestPlayer;
            newTestPlayer = PhotonNetwork.Instantiate("TestPlayer",
                    new Vector3 (0,0,0), Quaternion.identity).GetComponent<PlayerManager>();

            TestGround.SetActive(true);
        }

        public void Connect() => PhotonNetwork.ConnectUsingSettings();


        public void DisConnect() => PhotonNetwork.Disconnect();
        #endregion

        public void JoinLobby() => PhotonNetwork.JoinLobby();

        public void SetUser()
        {
            PlayerIndex = PhotonNetwork.PlayerList.Length - 1;
            UIManager.GetComponent<UIManager>().SetUserName(PlayerIndex, PhotonNetwork.LocalPlayer.NickName);
        }



        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            ChattingManager.instance.AddChatMessage(PhotonNetwork.LocalPlayer.NickName + " 님이 방을 나가셨습니다.");
        }


        public override void OnCreatedRoom()
        {
            print("방 만들기 완료");
            RoomManager.GetComponent<RoomManager>().AddRoom(PhotonNetwork.CurrentRoom.ToString());
        }
            
        public override void OnJoinedRoom()
        {
            Debug.LogFormat("방 참가 완료 : {0}", PhotonNetwork.CurrentRoom);
            if (PhotonNetwork.IsMasterClient == true)
            {
                UIManager.GetComponent<UIManager>().ToggleMaster();
                Debug.Log("마스터입니다!");
            }
            UIManager.GetComponent<UIManager>().TogglePanel(EGameState.ROOM);
            PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
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
            MapGenerator.Instance.GenerateMap();
            Photonview.RPC("RPC_StartGame", RpcTarget.All);
        }

        [PunRPC]
        void RPC_StartGame()
        {
            Debug.Log("스타트게임");
            //SceneManager.LoadScene("MainScene");
        }

        public void Ready()
        {
            if (!isReady) UIManager.GetComponent<UIManager>().GetReady(PlayerIndex);
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

    }
}