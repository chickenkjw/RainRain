using UnityEngine;
using Network;

namespace Game.Camera
{
    public class CameraController : MonoBehaviour
    {
        [Header("카메라 컨트롤러 파라미터")]
        
        [Tooltip("카메라 타겟. 플레이어")]
        public Transform target;
        
        [SerializeField] 
        [Tooltip("카메라 이동의 부드러운 정도")]
        [Range(1.0f, 8.0f)]
        private float smoothing;

        [HideInInspector]
        public bool followMouse;
        public Vector3 imaginaryObject;

        private void Start() {
            followMouse = false;
        }

        private void LateUpdate() {
            // 플레이어가 죽었을 경우, playerObject를 찾지 못해 null error가 나는 경우를 방지
            if (target == null) {
                try {
                    target = NetworkManager.instance.LocalPlayer.transform;
                }
                catch {
                    return;
                }
                
                return;
            }

            Vector3 targetPos = followMouse ? 
                new Vector3(imaginaryObject.x, imaginaryObject.y, transform.position.z) : 
                new Vector3(target.position.x, target.position.y, transform.position.z);
            
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothing * Time.deltaTime); 
        }
    }
}