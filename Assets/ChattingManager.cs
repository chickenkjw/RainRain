using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ChattingManager : MonoBehaviourPunCallbacks
{
    public GameObject ChattingContent;
    public InputField ChattingInput;

    PhotonView photonview;

    GameObject m_ContentText;

    string m_strUserName;


    void Start()
    {
        Screen.SetResolution(960, 600, false);
        PhotonNetwork.ConnectUsingSettings();
        m_ContentText = ChattingContent.transform.GetChild(0).gameObject;
        photonview = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && ChattingInput.isFocused == false)
        {
            Debug.Log("엔터 누름!");
            ChattingInput.ActivateInputField();
        }
        //if (Input.GetKeyDown(KeyCode.Return) && ChattingInput.isFocused == false)
        //{
        //    ChattingInput.ActivateInputField();
        //}
    }
    //public override void OnConnectedToMaster()
    //{
    //    RoomOptions options = new RoomOptions();
    //    options.MaxPlayers = 5;

    //    int nRandomKey = Random.Range(0, 100);

    //    m_strUserName = "user" + nRandomKey;

    //    PhotonNetwork.LocalPlayer.NickName = m_strUserName;
    //    PhotonNetwork.JoinOrCreateRoom("Room1", options, null);
    //}

    public override void OnJoinedRoom()
    {
        AddChatMessage("connect user : " + PhotonNetwork.LocalPlayer.NickName);
    }

    public void OnEndEditEvent()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Typed");
            string strMessage = PhotonNetwork.LocalPlayer.NickName + " : " + ChattingInput.text;

            photonview.RPC("RPC_Chat", RpcTarget.All, strMessage);
            ChattingInput.text = "";
        }
    }

    public void OnClickSendButton()
    {
        Debug.Log("Typed");
        string strMessage = PhotonNetwork.LocalPlayer.NickName + " : " + ChattingInput.text;

        photonview.RPC("RPC_Chat", RpcTarget.All, strMessage);
        ChattingInput.text = "";
    }

    void AddChatMessage(string message)
    {
        GameObject goText = Instantiate(m_ContentText, ChattingContent.transform);

        goText.GetComponent<Text>().text = message;
        ChattingContent.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

    }

    [PunRPC]
    void RPC_Chat(string message)
    {
        AddChatMessage(message);
    }

}