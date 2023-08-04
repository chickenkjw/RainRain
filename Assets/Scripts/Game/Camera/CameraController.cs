using System.Linq;
using Game.Player;
using UnityEngine;
using Network;

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
        [Range(1.0f, 8.0f)]
        private float smoothing;

        private void Start() {
            //target = NetworkManager.instance.LocalPlayer.transform;
            /*target = GameObject
                .FindGameObjectsWithTag("Player")
                .ToList().First(player => player.GetComponent<PlayerManager>().IsLocalPlayer)
                .GetComponent<Transform>();*/
        }

        private void LateUpdate() {
            // 플레이어가 죽었을 경우, playerObject를 찾지 못해 null error가 나는 경우를 방지
            if (target == null) {
                try
                {
                    target = NetworkManager.instance.LocalPlayer.transform;
                }
                catch
                {
                    Debug.LogError("카메라가 타겟을 못찾겠대요");
                }
                return;
            }
            
            Vector3 targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothing * Time.deltaTime);
        }
    }
}