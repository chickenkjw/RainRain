using System.Collections.Generic;
using System.Linq;
using Game.Fields;
using Game.Items;
using Photon.Pun;
using UnityEngine;

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
        public bool IsLocalPlayer { get; private set; }
        
        [SerializeField]
        [Tooltip("플레이어의 이동 속도")]
        private float moveSpeed;

        [Header("UI 오브젝트")]
        
        [SerializeField] 
        [Tooltip("플레이어의 인벤토리 UI")]
        private GameObject inventoryUI;

        [SerializeField] 
        [Tooltip("상자의 내용물 UI")]
        private GameObject boxContentsUI;

        [SerializeField] 
        [Tooltip("인벤토리가 켜질 때 주위 어두워지는거")]
        private GameObject uiBackground;
        
        #endregion

        #region Private Fields

        // 하단의 변수들은 에디터 상에 노출되지 않습니다
        private bool _canMoveVertical;
        private bool _canMoveUp;
        private bool _isMovingVertically;
        private Vector3 _stairDestination;
        
        private Item[] _playerItems;
        private Item[] _boxItems;

        [SerializeField]
        private GameObject[] _inventoryItemDrawPoint;
        [SerializeField]
        private Transform[] _boxItemDrawPoint;

        [SerializeField]
        private Transform _inventoryObjectsParent;
        [SerializeField]
        private Transform _itemObjectsParent;

        private bool _canInteractWithBox;
        private bool _isOpeningInventory;

        private List<Item> _items;
        
        #endregion

        #region MonoBehaviour CallBacks

        public void Awake()
        {
            IsLocalPlayer = PV.IsMine;
        }

        void Start()
        {
            SetVariables();

            inventoryUI.SetActive(false);
            boxContentsUI.SetActive(false);
            uiBackground.SetActive(false);

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

            _inventoryItemDrawPoint = new GameObject[4];
            _boxItemDrawPoint = new Transform[2];

            _items = MapGenerator.Instance.items;
            
            var playerUI = GameObject.FindGameObjectWithTag("PlayerUI");

            uiBackground = playerUI.transform.GetChild(0).gameObject;
            inventoryUI = playerUI.transform.GetChild(1).gameObject;
            boxContentsUI = playerUI.transform.GetChild(2).gameObject;

            for (int i = 0; i < 4; i++) {
                _inventoryItemDrawPoint[i] = inventoryUI.transform.GetChild(i).gameObject;
            }

            _boxItemDrawPoint[0] = boxContentsUI.transform.GetChild(0);
            _boxItemDrawPoint[1] = boxContentsUI.transform.GetChild(1);

            _inventoryObjectsParent = MapGenerator.Instance.drawObjectParents[0];
            _itemObjectsParent = MapGenerator.Instance.drawObjectParents[1];
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
            
            var rectTransform = inventoryUI.GetComponent<RectTransform>();
            
            if (!_isOpeningInventory) {
                _isOpeningInventory = true;
                
                uiBackground.SetActive(true);

                // 박스와 인벤토리 동시 열기
                if (_canInteractWithBox) {
                    rectTransform.anchoredPosition += Vector2.left * 105;
                    inventoryUI.SetActive(true);
                    boxContentsUI.SetActive(true);
                    DrawItemsOnInventory(true);
                }
                // 개인 인벤토리 열기
                else {
                    rectTransform.anchoredPosition = Vector2.zero;
                    inventoryUI.SetActive(true);
                    DrawItemsOnInventory();
                }
            }
            else {
                rectTransform.anchoredPosition = Vector2.zero;
                
                _isOpeningInventory = false;
                
                EraseItemsOnInventory(_canInteractWithBox);

                uiBackground.SetActive(false);
                inventoryUI.SetActive(false);
                boxContentsUI.SetActive(false);
            }
        }

        /// <summary>
        /// inventory UI를 활성화할 때, 아이템을 그림
        /// </summary>
        /// <param name="isBoxInteraction"></param>
        private void DrawItemsOnInventory(bool isBoxInteraction = false) {
            for (int i = 0; i < 4; i++) {
                if(_playerItems[i] != null)
                    Instantiate(_items.First(x => x.Equals(_playerItems[i])).gameObject, 
                        _inventoryItemDrawPoint[i].transform.position, Quaternion.identity, _inventoryObjectsParent);
            }
            
            if (isBoxInteraction) {
                for (int i = 0; i < 2; i++) {
                    if (_boxItems[i]  != null) {
                        Instantiate(_items.First(x => x.Equals(_boxItems[i])).gameObject,
                            _boxItemDrawPoint[i].transform.position, Quaternion.identity, _itemObjectsParent);
                    }
                }
            }
        }

        /// <summary>
        /// inventory UI를 비활성화 시킬 때, 표시된 아이템을 지움
        /// </summary>
        /// <param name="isInteractWithBox"></param>
        private void EraseItemsOnInventory(bool isInteractWithBox = false) {
            var drawItems = _inventoryObjectsParent.GetComponentsInChildren<Transform>();

            for (int i = 1; i < drawItems.Length; i++) {
                Destroy(drawItems[i].gameObject);
            }

            if (isInteractWithBox) {
                var boxItems = _itemObjectsParent.GetComponentsInChildren<Transform>();

                for (int i = 1; i < boxItems.Length; i++) {
                    Destroy(boxItems[i].gameObject);
                }
            }
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