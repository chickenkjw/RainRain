using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Network
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        public Text StatusText, RoomText;
        public InputField RoomInput, NickNameInput;
        public GameObject UIManager;
        public static NetworkManager instance;
        public GameObject LocalPlayerPrefab;
        public PhotonView Photonview;

        public PlayerManager LocalPlayer;
        #endregion

        #region Private Fields


        #endregion

        #region MonoBehaviour CallBacks
        void Awake()
        {
            instance = this;
            Screen.SetResolution(960, 540, false);
            //Connect();
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
            Debug.Log("���� ���� �Ϸ�");
            //PhotonNetwork.JoinLobby();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogFormat("���� ����, ���� : {0}", cause);
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("�κ� ���� �Ϸ�");
            UIManager.GetComponent<UIManager>().TogglePanel(EGameState.LOBBY);


        }
        #endregion

        #region Public Methods

        //�� ����� �� ����
        public void CreateRoom() => PhotonNetwork.CreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 10 });
        public void JoinRoom() => PhotonNetwork.JoinRoom(RoomInput.text);
        public void JoinOrCreateRoom()
        {
            PhotonNetwork.JoinOrCreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 10 }, null);
            UIManager.GetComponent<UIManager>().TogglePanel(EGameState.ROOM);
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

        public void GeneratePlayer(string name)
        {
            PlayerManager newPlayer;
            newPlayer = PhotonNetwork.Instantiate("Player",
                    new Vector3(0, 0, 0), Quaternion.identity).GetComponent<PlayerManager>();
            newPlayer.SetName(name);
            LocalPlayer = newPlayer;
        }

        public void Connect() => PhotonNetwork.ConnectUsingSettings();


        public void DisConnect() => PhotonNetwork.Disconnect();
        #endregion

        public void JoinLobby() => PhotonNetwork.JoinLobby();

        public void SetUser()
        {
            UIManager.GetComponent<UIManager>().SetUserName(PhotonNetwork.PlayerList.Length - 1, PhotonNetwork.LocalPlayer.NickName);
        }



        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            //RoomText.text = "Not in the room";
            Debug.Log(PhotonNetwork.NetworkClientState.ToString());
        }


        public override void OnCreatedRoom() => print("�� ����� �Ϸ�");
        public override void OnJoinedRoom()
        {
            Debug.LogFormat("�� ���� �Ϸ� : {0}", PhotonNetwork.CurrentRoom);
            if (PhotonNetwork.IsMasterClient == true)
            {
                UIManager.GetComponent<UIManager>().ToggleMaster();
                Debug.Log("�������Դϴ�!");
            }
            RoomText.text = PhotonNetwork.CurrentRoom.ToString();
            UIManager.GetComponent<UIManager>().TogglePanel(EGameState.ROOM);
            PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
            SetUser();
            //GeneratePlayer(NickNameInput.text);
        }
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("�� ����� ����, ���� : {0} : {1}", returnCode, message);
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("�� ���� ����, ���� : {0} : {1}", returnCode, message);
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            print("�� �������� ����");
        }


        public void SendChatting(string text)
        {
            //Photonview.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + text);
        }

        public void StartGame()
        {
            SceneManager.LoadScene("MainScene");
        }



        [ContextMenu("����")]
        void Info()
        {
            if (PhotonNetwork.InRoom)
            {
                print("���� �� �̸� : " + PhotonNetwork.CurrentRoom.Name);
                print("���� �� �ο��� : " + PhotonNetwork.CurrentRoom.PlayerCount);
                print("���� �� �ִ��ο��� : " + PhotonNetwork.CurrentRoom.MaxPlayers);

                string playerStr = "�濡 �ִ� �÷��̾� ��� : ";
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
                }
                print(playerStr);
            }
            else
            {
                print("������ �ο� ��  : " + PhotonNetwork.CountOfPlayers);
                print("�� ���� : " + PhotonNetwork.CountOfRooms);
                print("��� �濡 �ִ� �ο� ��  : " + PhotonNetwork.CountOfPlayersInRooms);
                print("�κ� �ִ���? : " + PhotonNetwork.InLobby);
                print("���� �ƴ���? : " + PhotonNetwork.IsConnected);
            }
        }

        [ContextMenu("����")]
        void Status()
        {
            Debug.Log(PhotonNetwork.NetworkClientState);
        }

        [ContextMenu("�ο� üũ")]
        void HeadCount()
        {
            string playerStr = "";
            print("�ο� ��  : " + PhotonNetwork.CurrentRoom.PlayerCount);
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                playerStr += PhotonNetwork.PlayerList[i].NickName + " : ";
            }
            print(playerStr);
        }

    }
}