using Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanelManager : MonoBehaviour
{
    public Text roomPlayers, roomName, roomHost;
    public string roomNameInfo;

    public void setInfo(int count, string roomname, string roomhost)
    {
        roomPlayers.text = count + "/4";
        roomName.text = roomname;
        roomHost.text = roomhost;

        roomNameInfo = roomname;
    }

    public void JoinRoomWithButton()
    {
        NetworkManager.instance.JoinRoom(roomNameInfo);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
