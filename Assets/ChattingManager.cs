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
    public GameObject m_ContentText;
    public static ChattingManager instance;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        photonview = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && ChattingInput.isFocused == false)
        {
            ChattingInput.ActivateInputField();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("채팅");
            string strMessage = PhotonNetwork.LocalPlayer.NickName + " : " + ChattingInput.text;
            photonview.RPC("RPC_Chat", RpcTarget.All, strMessage);
            ChattingInput.text = "";
        }
    }


    //public override void OnJoinedRoom()
    //{
        
    //}

    public void OnEndEditEvent()
    {

    }

    public void OnClickSendButton()
    {
        string strMessage = PhotonNetwork.LocalPlayer.NickName + " : " + ChattingInput.text;
        ChattingInput.text = "";
        photonview.RPC("RPC_Chat", RpcTarget.All, strMessage);
    }

    public void AddChatMessage(string message)
    {
        photonview.RPC("RPC_Chat", RpcTarget.All, message);
        //GameObject goText = Instantiate(m_ContentText, ChattingContent.transform);

        //goText.GetComponent<Text>().text = message;
        //ChattingContent.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

    }


    [PunRPC]
    void RPC_Chat(string message)
    {
        GameObject goText = Instantiate(m_ContentText, ChattingContent.transform);

        goText.GetComponent<Text>().text = message;
        ChattingContent.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        //AddChatMessage(message);
    }

    [ContextMenu("채팅 클리어")]
    public void ClearChatting()
    {
        foreach (Transform chat in ChattingContent.GetComponentsInChildren<Transform>())
        {
            Destroy(chat.gameObject);
        }
    }

}