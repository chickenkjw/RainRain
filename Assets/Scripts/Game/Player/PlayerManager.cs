using System.Collections.Generic;
using Game.Items;
using Photon.Pun;
using UnityEngine;

namespace Game.Player
{
    public class PlayerManager : MonoBehaviour
    {
        #region Public Fields

        public PhotonView PV;
        public TextMesh PlayerName;

        #endregion

        #region Private Fields

        [SerializeField]
        [Tooltip("플레이어가 로컬 플레이어인지 확인")]
        private bool isLocalPlayer;

        [SerializeField]
        [Tooltip("플레이어의 이동 속도")]
        private float moveSpeed;
        
        // 하단의 변수들은 에디터 상에 노출되지 않습니다
        private bool _canMoveVertical;
        private bool _canMoveUp;
        private bool _isMovingVertically;
        private Vector3 _stairDestination;
        
        public List<Item> Items;

        #endregion

        #region MonoBehaviour CallBacks

        public void Awake()
        {
            isLocalPlayer = PV.IsMine;
        }

        void Start()
        {
            _canMoveVertical = false;
            _canMoveUp = false;
            _isMovingVertically = false;

            _stairDestination = Vector3.zero;
            Debug.LogFormat("플레이어 입장 : {0}", PhotonNetwork.LocalPlayer.NickName);
            Items = new();

            SetName(PhotonNetwork.LocalPlayer.NickName);
        }

        // Update is called once per frame
        void Update()
        {
            if (isLocalPlayer) {
                Move();
                MoveVertical();
            }
        }

        #endregion


        #region Public Methods
        
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
        
        /// <summary>
        /// Player 좌우 이동 함수
        /// </summary>
        private void Move()
        {
            float moveInput = Input.GetAxis("Horizontal");
            float moveAmount = moveInput * moveSpeed * Time.deltaTime;
            transform.Translate(Vector3.right * moveAmount);
        }
        
        /// <summary>
        /// Player 층간 이동 함수
        /// </summary>
        private void MoveVertical() {
            if (!_canMoveVertical) {
                return;
            }

            float v = Input.GetAxis("Vertical");

            if (v != 0 && (_canMoveUp && v > 0 || !_canMoveUp && v < 0)) {
                _isMovingVertically = true;
                transform.position = _stairDestination;
            }
        }

        /// <summary>
        /// 물에 닿았을 시 플레이어가 죽는 것으로 판정
        /// </summary>
        public void Drawn() {
            GameManager.Instance.PlayerDie(PlayerName.text);
            Destroy(this.gameObject);
        } 

        private void OnTriggerEnter2D(Collider2D other) {
            var obj = other.gameObject;

            if (obj.CompareTag("StairPoint")) {
                _canMoveVertical = true;
                var parentObj = obj.GetComponentInParent<Transform>().parent;

                if (obj.name.Equals("UpStair")) {
                    _canMoveUp = true;
                    _stairDestination = parentObj.GetChild(3).position;
                }
                else {
                    _canMoveUp = false;
                    _stairDestination = parentObj.GetChild(2).position;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            var obj = other.gameObject;

            if (obj.CompareTag("StairPoint")) {
                if (_isMovingVertically) {
                    _isMovingVertically = false;
                    return;
                }
                _canMoveVertical = false;
            }
        }

        public void SetName(string name)
        {
            Debug.Log(name + "으로 세팅하겠습니다!");
            PV.RPC("SetNameRPC", RpcTarget.AllBuffered, name);
        }
        [PunRPC]
        public void SetNameRPC(string name)
        {
            PlayerName.text = name;
        }

        #endregion
    }

}