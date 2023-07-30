using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class EnvironmentManager : MonoBehaviour
{
    [Tooltip("자신의 Photon View 파일")]
    public PhotonView PV;


    public static EnvironmentManager _instance;
    public static EnvironmentManager Instance
    {
        get
        {
            if (_instance == null)
            {
                return null;
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
