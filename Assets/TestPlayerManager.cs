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

        [Header("플레이어 데이터")]

        [Tooltip("자신의 Photon View 파일")]
        public PhotonView PV;

        //[HideInInspector]
        public TextMesh PlayerName;

        [Tooltip("플레이어가 로컬 플레이어인지 확인")]
        public bool IsLocalPlayer { get; private set; }

        [SerializeField]
        [Tooltip("플레이어의 이동 속도")]
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
        /// 나중에 쓸지 모르니 일단 만든 함수
        /// </summary>


        /// <summary>
        /// Player 좌우 이동 함수
        /// </summary>
        private void Move()
        {
            float moveInput = Input.GetAxis("Horizontal");
            float moveAmount = moveInput * moveSpeed * Time.deltaTime;
            transform.Translate(Vector3.right * moveAmount);
        }


        private void SetName(string name)
        {
            Debug.Log(name + "으로 세팅하겠습니다!");
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