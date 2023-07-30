using Game.Items;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Player
{
    public class PlayerManager : MonoBehaviour
    {
        #region Public Fields
        
        [Header("플레이어 데이터")]

        [Tooltip("자신의 Photon View 파일")]
        public PhotonView PV;
        
        [HideInInspector]
        public TextMesh PlayerName;

        [Tooltip("플레이어가 로컬 플레이어인지 확인")]
        public bool IsLocalPlayer;
        
        [SerializeField]
        [Tooltip("플레이어의 이동 속도")]
        private float moveSpeed;

        [FormerlySerializedAs("_playerUIManager")]
        [SerializeField] 
        [Tooltip("PlayerUIManager")]
        private ItemUIManager itemUIManager;

        #endregion

        #region Private Fields

        // 하단의 변수들은 에디터 상에 노출되지 않습니다
        
        // 플레이어의 이동과 관련된 변수
        private bool _canMoveVertical;
        private bool _canMoveUp;
        private bool _isMovingVertically;
        private Vector3 _stairDestination;
        
        // 플레이어의 아이템을 담는 변수
        private Item[] _playerItems;
        private Item[] _boxItems;
        
        // 인벤토리 열 수 있는 지에 대한 변수
        private bool _canInteractWithBox;
        [SerializeField]
        private bool _isOpeningInventory;

        #endregion

        #region MonoBehaviour CallBacks

        public void Awake()
        {
            // IsLocalPlayer = PV.IsMine;
            IsLocalPlayer = true;
        }

        void Start()
        {
            SetVariables();
            
            SetName(PhotonNetwork.LocalPlayer.NickName);
        }
        
        void Update()
        {
            if (IsLocalPlayer) {
                Move();
                MoveVertical();
                OpenInventory();
            }
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// 나중에 쓸지 모르니 일단 만든 함수
        /// </summary>
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

        private void SetVariables() {
            _canMoveVertical = false;
            _canMoveUp = false;
            _isMovingVertically = false;

            _canInteractWithBox = false;
            _isOpeningInventory = false;
            
            _stairDestination = Vector3.zero;
            
            _playerItems = new Item[4];
            _boxItems = new Item[2];
        }
        
        /// <summary>
        /// Player 좌우 이동 함수
        /// </summary>
        private void Move()
        {
            if (_isOpeningInventory) {
                return;
            }
            
            float moveInput = Input.GetAxis("Horizontal");
            float moveAmount = moveInput * moveSpeed * Time.deltaTime;
            transform.Translate(Vector3.right * moveAmount);
        }

        /// <summary>
        /// Player 층간 이동 함수
        /// </summary>
        private void MoveVertical() {
            if (!_canMoveVertical || _isOpeningInventory) {
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
        public void Drown() {
            GameManager.Instance.PlayerDie(PlayerName.text);
            Destroy(gameObject);
        }

        private void OpenInventory() {
            if (!Input.GetKeyDown(KeyCode.E)) {
                return;
            }
            
            _isOpeningInventory = itemUIManager
                .OpenInventory(_canInteractWithBox, ref _playerItems, ref _boxItems);
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
            else if (obj.CompareTag("Box")) {
                _canInteractWithBox = true;
                var item = obj.GetComponent<BoxContents>();
                _boxItems[0] = item.item1;
                _boxItems[1] = item.item2;
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
            else if (obj.CompareTag("Box")) {
                _canInteractWithBox = false;
                _boxItems[0] = null;
                _boxItems[1] = null;
            }
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