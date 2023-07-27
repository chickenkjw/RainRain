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

        #endregion

        #region Private Fields

        [SerializeField]
        [Tooltip("���� ������Ʈ�� ���� �÷��̾����� ����")]
        private bool isLocalPlayer;

        [SerializeField]
        [Tooltip("�÷��̾��� �̵��ӵ�")]
        private float moveSpeed;
        
        private bool _canMoveVertical;
        private bool _canMoveUp;
        private bool _isMovingVertically;
        private Vector3 _stairDestination;

        public List<Item> Items;

        #endregion
    
        #region MonoBehaviour CallBacks

        private void Awake()
        {
            // isLocalPlayer = PV.IsMine;
            isLocalPlayer = true;
        }

        private void Start() {
            _canMoveVertical = false;
            _canMoveUp = false;
            _isMovingVertically = false;

            _stairDestination = Vector3.zero;

            Items = new();
        }

        void Update()
        {
            if (isLocalPlayer) {
                Move();
                MoveVertical();
            }
        }

        #endregion

        /// <summary>
        /// Player �¿� �̵� �Լ�
        /// </summary>
        private void Move()
        {
            float moveInput = Input.GetAxis("Horizontal");
            float moveAmount = moveInput * moveSpeed * Time.deltaTime;
            transform.Translate(Vector3.right * moveAmount);
        }

        /// <summary>
        /// Player ���� �̵� �Լ�
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

        public void Drawn() {
            Debug.Log("���ƾ�");
            GameManager.Instance.PlayerDie("temp");
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
    }
}