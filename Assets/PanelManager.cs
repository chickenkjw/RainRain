using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UserStatus
{
    Master,
    NotReady,
    Ready
}

public class PanelManager : MonoBehaviour
{
    public Text PlayerName;
    public List<GameObject> IconList;
    //public void ClearIcon()
    //{
    //    foreach (GameObject icon in IconList)
    //    {
    //        icon.SetActive(false);
    //    }
    //}

    public void SetIcon(UserStatus status)
    {
        Debug.Log("아이콘 세팅");
        ClearIcon();
        switch (status)
        {
            case UserStatus.Master:
                IconList[0].SetActive(true);
                break;
            case UserStatus.NotReady:
                IconList[1].SetActive(true);
                break;
            case UserStatus.Ready:
                IconList[2].SetActive(true);
                break;
        }
    }

    public void SetUsername(string username)
    {
        this.gameObject.SetActive(true);
        PlayerName.text = username;
    }
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    [ContextMenu("패널 클리어")]
    public void ClearIcon()
    {
        foreach (GameObject icon in IconList)
        {
            icon.SetActive(false);
        }
    }
}
