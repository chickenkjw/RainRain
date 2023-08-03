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

    public List<PanelManager> UserPanelList;

    public GameObject ReadyButton;
    public GameObject StartGameButton;

    public PhotonView PV;

    public static UIManager instance;

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

    void Start()
    {

    }
    
    #endregion
    
    #region Public Methods

    public void ClearPanel()
    {
        MainPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
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
