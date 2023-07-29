using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviour
{
    public PhotonView PV;
    public GameObject RoomContent;
    public GameObject RoomPanel;
    public void AddRoom (string roomInfo)
    {
        Debug.Log(roomInfo);
        PV.RPC("RPC_AddRoom", RpcTarget.AllBuffered, roomInfo);
    }

    void OnAddRoom(string roomInfo)
    {
        GameObject newRoomPanel = Instantiate(RoomPanel, RoomContent.transform);

        newRoomPanel.GetComponentInChildren<Text>().text = roomInfo; 
        RoomContent.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

    }

    [PunRPC]
    void RPC_AddRoom(string roomInfo)
    {
        Debug.Log("¿©±â±îÁö ¿È");
        //OnAddRoom(roomInfo);
    }
}
