using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Game;
using Game.Player;
using Game.Fields;
using UnityEngine.SceneManagement;

namespace Network
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        public Text StatusText;
        public InputField RoomInput, NickNameInput;
        //public GameObject UIManager;
        public GameObject RoomManager;
        public static NetworkManager instance;
        public GameObject LocalPlayerPrefab;
        public PhotonView Photonview;
        public GameObject TestGround;

        public GameObject LocalPlayer;
        public int PlayerIndex { get; private set; }
        public bool isReady;
        #endregion

        #region Private Fields


        #endregion

        #region MonoBehaviour CallBacks
        void Awake()
        {
            // SoundManager �ν��Ͻ��� �̹� �ִ��� Ȯ��, �� ���·� ����
            if (instance == null)
                instance = this;

            // �ν��Ͻ��� �̹� �ִ� ��� ������Ʈ ����
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
            UIManager.instance.TogglePanel(EGameState.LOBBY);
        }
        #endregion

        #region Public Methods

        //�� ����� �� ����
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
            Debug.Log("�׽�Ʈ �÷��̾� ����!");
            PlayerManager newTestPlayer;
            newTestPlayer = PhotonNetwork.Instantiate("TestPlayer",
                    new Vector3 (0,0,0), Quaternion.identity).GetComponent<PlayerManager>();

            TestGround.SetActive(true);
        }

        public GameObject GenerateEnvironment()
        {
            GameObject environment = PhotonNetwork.Instantiate("Environment",
                    new Vector3(0, 0, 0), Quaternion.identity);
            return environment;
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
            ChattingManager.instance.AddChatMessage(PhotonNetwork.LocalPlayer.NickName + " ���� ���� �����̽��ϴ�.");
        }


        public override void OnCreatedRoom()
        {
            print("�� ����� �Ϸ�");
            RoomManager.GetComponent<RoomManager>().AddRoom(PhotonNetwork.CurrentRoom.ToString());
        }
            
        public override void OnJoinedRoom()
        {
            Debug.LogFormat("�� ���� �Ϸ� : {0}", PhotonNetwork.CurrentRoom);
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
            UIManager.instance.SetRoomInfo(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CurrentRoom.PlayerCount);
            

            if (PhotonNetwork.IsMasterClient == true)
            {
                UIManager.instance.ToggleMaster();
            }
            UIManager.instance.TogglePanel(EGameState.ROOM);

            PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
            ChattingManager.instance.AddChatMessage(PhotonNetwork.LocalPlayer.NickName + " ���� �����ϼ̽��ϴ�");
            SetUser();
            GenerateTestPlayer();
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
            //MapGenerator.Instance.GenerateMap();
            Photonview.RPC("RPC_StartGame", RpcTarget.All);
        }

        [PunRPC]
        void RPC_StartGame()
        {
            Debug.Log("��ŸƮ����");
            //DeleteObjectsWithTag("TestPlayer");
            SceneManager.LoadScene("MainScene");
        }

        public void Ready()
        {
            Debug.Log("����");
            //������ ����� ��ɵ� �߰��� ��
            if (!isReady)
            {
                UIManager.instance.GetReady(PlayerIndex);
            }
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