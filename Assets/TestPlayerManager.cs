using System.Collections.Generic;
using System.Linq;
using Game.Fields;
using Game.Items;
using Photon.Pun;
using UnityEngine;

namespace Game.Player
{
    public class TestPlayerManager : MonoBehaviour
    {
        #region Public Fields

        [Header("�÷��̾� ������")]

        [Tooltip("�ڽ��� Photon View ����")]
        public PhotonView PV;

        //[HideInInspector]
        public TextMesh PlayerName;

        [Tooltip("�÷��̾ ���� �÷��̾����� Ȯ��")]
        public bool IsLocalPlayer { get; private set; }

        [SerializeField]
        [Tooltip("�÷��̾��� �̵� �ӵ�")]
        private float moveSpeed;

        #endregion

        #region Private Fields



        #endregion

        #region MonoBehaviour CallBacks

        public void Awake()
        {
            IsLocalPlayer = PV.IsMine;
        }

        void Start()
        {
            SetName(PhotonNetwork.LocalPlayer.NickName);
        }

        void Update()
        {
            if (IsLocalPlayer)
            {
                Move();

            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// ���߿� ���� �𸣴� �ϴ� ���� �Լ�
        /// </summary>


        /// <summary>
        /// Player �¿� �̵� �Լ�
        /// </summary>
        private void Move()
        {
            float moveInput = Input.GetAxis("Horizontal");
            float moveAmount = moveInput * moveSpeed * Time.deltaTime;
            transform.Translate(Vector3.right * moveAmount);
        }


        private void SetName(string name)
        {
            Debug.Log(name + "���� �����ϰڽ��ϴ�!");
            PV.RPC(nameof(SetNameRPC), RpcTarget.AllBuffered, name);
        }

        [PunRPC]
        public void SetNameRPC(string name)
        {
            PlayerName.text = name;
        }

        #endregion
    }

}