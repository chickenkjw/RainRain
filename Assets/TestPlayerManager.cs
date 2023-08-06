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
        public bool isLocalPlayer { get; private set; }

        [SerializeField]
        [Tooltip("플레이어의 이동 속도")]
        private float moveSpeed;

        #endregion

        #region Private Fields

        private float moveInput;
        
        // 애니메이터
        public Animator playerAnimator;
        public Animator clothAnimator;

        public SpriteRenderer playerRenderer;
        public SpriteRenderer clothRenderer;

        #endregion

        #region MonoBehaviour CallBacks

        public void Awake()
        {
            isLocalPlayer = PV.IsMine;
        }

        void Start()
        {
            if(isLocalPlayer) 
                SetName(PhotonNetwork.LocalPlayer.NickName);
            moveInput = 0f;
            
            playerAnimator = GetComponent<Animator>();
            clothAnimator = transform.GetChild(2).GetComponent<Animator>();

            playerRenderer = GetComponent<SpriteRenderer>();
            clothRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (isLocalPlayer)
            {
                Move();
                Anim();
            }
        }

        #endregion

        #region Public Methods

        private void Anim() {
            var isMove = moveInput != 0;
            playerAnimator.SetBool("isMove", isMove);
            clothAnimator.SetBool("isMove", isMove);

            if (moveInput > 0 && isMove) {
                playerRenderer.flipX = true;
                clothRenderer.flipX = true;
            }
            else if (moveInput < 0 && isMove) {
                playerRenderer.flipX = false;
                clothRenderer.flipX = false;
            }
        }


        /// <summary>
        /// Player 좌우 이동 함수
        /// </summary>
        private void Move()
        {
            moveInput = Input.GetAxis("Horizontal");
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