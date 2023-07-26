using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using TMPro;


public class PlayerManager : MonoBehaviour
{
    #region Public Fields

    public PhotonView PV;
    public TextMesh PlayerName;

    #endregion

    #region Private Fields

    [SerializeField]
    [Tooltip("���� ������Ʈ�� ���� �÷��̾����� ����")]
    private bool isLocalPlayer;

    [SerializeField]
    [Tooltip("�÷��̾��� �̵��ӵ�")]
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


    #region Public Methods
    /*player �̵� ���� �Լ�*/
    public void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        float moveAmount = moveInput * moveSpeed * Time.deltaTime;
        transform.Translate(Vector3.right * moveAmount);
    }

    public void SetName(string name)
    {
        PV.RPC("SetNameRPC", RpcTarget.AllBuffered, name);
    }

    [PunRPC]
    public void SetNameRPC(string name)
    {
        PlayerName.text = name;
    }

    #endregion

}