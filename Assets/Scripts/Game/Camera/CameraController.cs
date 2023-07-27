using System.Linq;
using Game.Player;
using UnityEngine;

namespace Game.Camera
{
    public class CameraController : MonoBehaviour
    {
        [Header("카메라 컨트롤러 파라미터")]
        
        [SerializeField]
        [Tooltip("카메라 타겟. 플레이어")]
        private Transform target;
        
        [SerializeField] 
        [Tooltip("카메라 이동의 부드러운 정도")]
        private float smoothing = 0.2f;

        private void Start() {
            target = GameObject
                .FindGameObjectsWithTag("Player")
                .ToList().First(player => player.GetComponent<PlayerManager>().IsLocalPlayer)
                .GetComponent<Transform>();
        }

        private void LateUpdate() {
            Vector3 targetPos = new Vector3(target.position.x, target.position.y, this.transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
        }
    }
}