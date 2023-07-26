using Game.Player;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

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
            Debug.Log("���� ���� �Ϸ�");
            PhotonNetwork.JoinLobby();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogFormat("���� ����, ���� : {0}", cause);
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("�κ� ���� �Ϸ�");
        }
        #endregion

        #region Public Methods
        public void Join()
        {
            PhotonNetwork.JoinOrCreateRoom("����ٺ�", new RoomOptions { MaxPlayers = 6 }, null);
        }

        //�� ����� �� ����
        public void CreateRoom() => PhotonNetwork.CreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 5 });
        public void JoinRoom(string RoomName) => PhotonNetwork.JoinRoom(RoomName);
        public void JoinOrCreateRoom(string RoomName) => PhotonNetwork.JoinOrCreateRoom(RoomName, new RoomOptions { MaxPlayers = 5 }, null);
        public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();
        public void LeaveRoom() => PhotonNetwork.LeaveRoom();
        public void LeaveRoom(string RoomName)
        {
            PhotonNetwork.LeaveRoom();
        }

        public void GeneratePlayer(string name)
        {
            PlayerManager newPlayer;
            newPlayer = PhotonNetwork.Instantiate("Player",
                new Vector3(0, 0, 0), Quaternion.identity).GetComponent<PlayerManager>();
            //newPlayer.SetName(name);
            LocalPlayer = newPlayer;
        }

        public void Connect() => PhotonNetwork.ConnectUsingSettings();
        //public override void OnConnectedToMaster()
        //{
        //    Debug.Log("���� ���� �Ϸ�");
        //    //PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        //    UIManager.GetComponent<UIManager>().ShowControlPanel();
        //}


        public void DisConnect() => PhotonNetwork.Disconnect();
        #endregion



        public void JoinLobby() => PhotonNetwork.JoinLobby();




        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            RoomText.text = "Not in the room";
            Debug.Log(PhotonNetwork.NetworkClientState.ToString());
            //PhotonNetwork.JoinOrCreateRoom("Team1", new RoomOptions { MaxPlayers = 5 }, null);
        }


        public override void OnCreatedRoom() => print("�� ����� �Ϸ�");
        public override void OnJoinedRoom()
        {
            Debug.LogFormat("�� ���� �Ϸ� : {0}", PhotonNetwork.CurrentRoom);
            RoomText.text = PhotonNetwork.CurrentRoom.ToString();
            //UIManager.GetComponent<UIManager>().HideSimplePanel();
            GeneratePlayer("�׽�Ʈ");
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
                print("������ �ο� �� : " + PhotonNetwork.CountOfPlayers);
                print("�� ���� : " + PhotonNetwork.CountOfRooms);
                print("��� �濡 �ִ� �ο� �� : " + PhotonNetwork.CountOfPlayersInRooms);
                print("�κ� �ִ���? : " + PhotonNetwork.InLobby);
                print("����ƴ���? : " + PhotonNetwork.IsConnected);
            }
        }

        [ContextMenu("����")]
        void Status()
        {
            Debug.Log(PhotonNetwork.NetworkClientState);
        }

    }
}
