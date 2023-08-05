using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Network;

public class RoomManager : MonoBehaviour
{
    public GameObject RoomContent;
    public Transform RoomPanel;


    public static RoomManager instance;

    public void Awake()
    {
        instance = this;
    }



    public void AddRoom (int playerCount, string roomName, string roomHost)
    {
        GameObject newRoom = Instantiate(RoomContent, Vector3.zero, Quaternion.identity);
        newRoom.transform.SetParent(RoomPanel, false);

        newRoom.GetComponent<RoomPanelManager>().setInfo(playerCount, roomName, roomHost);
    }
}
