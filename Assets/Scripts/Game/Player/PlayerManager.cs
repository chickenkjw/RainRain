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

        /*player �̵� ���� �Լ�*/
        private void Move()
        {
            float moveInput = Input.GetAxis("Horizontal");
            float moveAmount = moveInput * moveSpeed * Time.deltaTime;
            transform.Translate(Vector3.right * moveAmount);
        }
    }
}