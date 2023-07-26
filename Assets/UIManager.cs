using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

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

    public List<GameObject> UserPanelList;

    public GameObject a;

    public PhotonView PV;

    #endregion
    #region MonoBehaviour CallBacks
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
        Debug.Log("I'm master");
    }

    public void SetUserName(int userindex, string username)
    {
        PV.RPC("SetUserNameRPC", RpcTarget.AllBuffered, userindex, username);
    } 

    [PunRPC]
    public void SetUserNameRPC(int userindex, string name)
    {
        UserPanelList[userindex].GetComponentInChildren<Text>().text = name;
    }


    #endregion
}
