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
        [Tooltip("현재 오브젝트가 로컬 플레이어인지 구분")]
        private bool isLocalPlayer;

        [SerializeField]
        [Tooltip("플레이어의 이동속도")]
        private float moveSpeed;

        #endregion
    
        #region MonoBehaviour CallBacks

        public void Awake()
        {
            isLocalPlayer = PV.IsMine;
        }

        void Update()
        {
            if(isLocalPlayer) Move();
        }

        #endregion

        /*player 이동 관련 함수*/
        private void Move()
        {
            float moveInput = Input.GetAxis("Horizontal");
            float moveAmount = moveInput * moveSpeed * Time.deltaTime;
            transform.Translate(Vector3.right * moveAmount);
        }
    }
}