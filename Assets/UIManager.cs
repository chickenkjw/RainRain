using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public enum EGameState
{
    MAIN,
    LOBBY,
    ROOM,
    GAME
};

public class UIManager : MonoBehaviour
{
    #region Public Fields

    public GameObject MainPanel;
    public GameObject LobbyPanel;
    public GameObject RoomPanel;
    public GameObject StatusPanel;
    public GameObject CreateRoomPanel;

    public List<PanelManager> UserPanelList;
    public List<ColorSelectManager> ColorSelectorList;

    public Text statusText;
    public Text RoomNameText;
    public Text UserCountText;
    public Text NicknameText;

    public GameObject ReadyButton;
    public GameObject StartGameButton;

    public PhotonView PV;

    public static UIManager instance;

    public Button entryButton;
    private bool connectedToMasterServer;

    #endregion
    
    #region MonoBehaviour CallBacks
   
    void Awake()
    {
        if (instance == null) {
            instance = this;
            
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            Destroy(this.gameObject);
        }
        
        Screen.SetResolution(960, 540, false);
        
    }

    void Start() {
        entryButton.gameObject.SetActive(false);
        connectedToMasterServer = false;
    }
    
    #endregion
    
    #region Public Methods

    public void SetStatus(string status)
    {
        statusText.text = status;
        if (!connectedToMasterServer) {
            connectedToMasterServer = status.Equals("ConnectedToMasterServer");
            entryButton.gameObject.SetActive(connectedToMasterServer);
        }
    }

    public void ClearPanel()
    {
        MainPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        StatusPanel.SetActive(false);
        CreateRoomPanel.SetActive(false);
    }

    public void OpenCreateRoomPanel()
    {
        CreateRoomPanel.SetActive(true);
    }

    public void CloseCreateRoomPanel()
    {
        CreateRoomPanel.SetActive(false);
    }

    public void TogglePanel(EGameState toggle)
    {
        ClearPanel();
        switch(toggle)
        {
            case EGameState.MAIN:
                MainPanel.SetActive(true);
                break;
            case EGameState.LOBBY:
                LobbyPanel.SetActive(true);
                break;
            case EGameState.ROOM:
                RoomPanel.SetActive(true);
                break;
        }
    }

    public void ToggleMaster()
    {
        ReadyButton.SetActive(false);
        StartGameButton.SetActive(true);
        Debug.Log("I'm master");
    }

    public void ClearColorSelector()
    {
        foreach(ColorSelectManager selector in ColorSelectorList)
        {
            selector.UnSelect();
        }
    }

    public void SetNickname(string nickname)
    {
        NicknameText.text = nickname;
    }

    public void SetRoomInfo(string roomname, int playercount)
    {
        RoomNameText.text = roomname;
        PV.RPC("SetUsercountRPC", RpcTarget.All, playercount);
    }

    [PunRPC]
    public void SetUsercountRPC(int playercount)
    {
        UserCountText.text = playercount + "/4";
    }

    public void SetUserName(int userindex, string username)
    {
        PV.RPC("SetUserNameRPC", RpcTarget.AllBuffered, userindex, username);
    } 

    [PunRPC]
    public void SetUserNameRPC(int userindex, string name)
    {
        UserPanelList[userindex].GetComponent<PanelManager>().SetUsername(name);
    }

    public void GetReady(int userIndex)
    {
        PV.RPC("GetReadyRPC", RpcTarget.AllBuffered, userIndex);
    }

    [PunRPC]
    public void GetReadyRPC(int userIndex)
    {
        UserPanelList[userIndex].SetIcon(UserStatus.Ready);
    }

    #endregion
    
    
}
