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


    void Start()
    {
        Screen.SetResolution(960, 600, false);
        PhotonNetwork.ConnectUsingSettings();
        photonview = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && ChattingInput.isFocused == false)
        {
            ChattingInput.ActivateInputField();
        }
    }


    public override void OnJoinedRoom()
    {
        photonview.RPC("RPC_Chat", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " ¥‘¿Ã ¡¢º”«œºÃΩ¿¥œ¥Ÿ");
    }

    public void OnEndEditEvent()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string strMessage = PhotonNetwork.LocalPlayer.NickName + " : " + ChattingInput.text;
            photonview.RPC("RPC_Chat", RpcTarget.All, strMessage);
            ChattingInput.text = "";
        }
    }

    public void OnClickSendButton()
    {
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