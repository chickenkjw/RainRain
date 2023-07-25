using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;


public class PlayerManager : MonoBehaviour
{
    #region Public Fields

    public PhotonView PV;

    #endregion

    #region Private Fields

    [SerializeField]
    [Tooltip("현재 오브젝트가 로컬 플레이어인지 구분")]
    private bool isLocalPlayer;

    [SerializeField]
    [Tooltip("플레이어의 이동속도")]
    private float moveSpeed;

    #endregion


    public void Set()
    {
        //isLocalPlayer = PV.IsMine;
        //PlayerCamera.enabled = isLocalPlayer;

        //GetComponentInChildren<Text>().text = name;

        //tpCamObject = tpCamera.gameObject;
        //tpRig = tpCamera.transform.parent;
        //tpRoot = tpRig.parent;

        //fpCamObject = fpCamera.gameObject;
        //fpRig = fpCamera.transform.parent;
        //fpRoot = fpRig.parent;

        //TryGetComponent(out rigid);
        //if (rigid != null)
        //{
        //    rigid.constraints = RigidbodyConstraints.FreezeRotation;
        //}

        //TryGetComponent(out CapsuleCollider cCol);
        //_groundCheckRadius = cCol ? cCol.radius : 0.1f;
        //animator = GetComponent<Animator>();
    }
    #region MonoBehaviour CallBacks

    public void Awake()
    {
        isLocalPlayer = PV.IsMine;
    }

    void Start()
    {
        Set();
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer) Move();
    }

    #endregion



    /*player 이동 관련 함수*/
    public void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        float moveAmount = moveInput * moveSpeed * Time.deltaTime;
        transform.Translate(Vector3.right * moveAmount);
    }

}