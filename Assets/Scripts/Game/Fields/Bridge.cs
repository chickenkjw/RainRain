using Game.Camera;
using Game.Items;
using Network;
using UnityEngine;
using Cam = UnityEngine.Camera;

namespace Game.Fields
{
    /// <summary>
    /// 마우스를 감지하는 오브젝트
    /// </summary>
    public class Bridge : MonoBehaviour
    {
        public Direction direction;
        public Location location;

        private int _boardCount;

        private GameObject _bridgeObject;
        private GameObject _createdObject;

        private CameraController camera;

        private Vector3 _mousePositionOffset;

        private CameraController _cameraController;

        private Vector3 curMousePos;
        private Vector3 prevMousePos;
        private Vector3 destination;
        
        private void Start() {
            var objectList = GameObject.Find("ItemManager").GetComponent<ItemManager>().bridgeObject;
           _bridgeObject = direction == Direction.Left ? objectList[1] : objectList[0];
        }

        private Vector3 GetMousePosition() {
            return Cam.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private Vector3 ScreenCenterToWorldPoint() {
            return Cam.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Cam.main.nearClipPlane));
        }

        private void OnMouseDown() {
            _cameraController = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();

            _bridgeObject.transform.localScale = new Vector3(.1f, .1f, .1f);

            _createdObject = Instantiate(_bridgeObject, transform.position, Quaternion.identity,
                MapGenerator.Instance.environmentObject.transform);

            _mousePositionOffset = transform.position - GetMousePosition();

            var createdBridgeObject = _createdObject.GetComponent<BridgeObject>();
            createdBridgeObject.parentTransform = this.transform;
            createdBridgeObject.Direction = direction;
            
            _cameraController.imaginaryObject = ScreenCenterToWorldPoint();
            _cameraController.followMouse = true;
        }

        private void OnMouseDrag() {
            Vector3 destination = GetMousePosition() + _mousePositionOffset;
            float distance = Vector2.Distance(this.destination, transform.position);
            distance /= 9.8f;

            _cameraController.imaginaryObject = ScreenCenterToWorldPoint();

            _createdObject.transform.localScale = new Vector3(distance, .3f, 1f);
            
            float angle = Mathf.Atan2(destination.y - transform.position.y, destination.x - transform.position.x) * Mathf.Rad2Deg;
            _createdObject.transform.rotation = Quaternion.Euler(0, 0, angle);

            prevMousePos = curMousePos;
        }

        private void OnMouseUp() {
            var bridgeObject = _createdObject.GetComponent<BridgeObject>();
            var parentTransform = bridgeObject.parentTransform;
            if (parentTransform != null && parentTransform.position != transform.position && bridgeObject.entriesCount == 1) {
                float distance = Vector2.Distance(parentTransform.position, transform.position);
                
                distance /= 9.8f;
                
                float angle = Mathf.Atan2(parentTransform.position.y - transform.position.y,
                                  parentTransform.position.x - transform.position.x)
                              * Mathf.Rad2Deg;
                _createdObject.transform.rotation = Quaternion.Euler(0, 0, angle);

                _createdObject.transform.localScale = new Vector3(distance, .3f, 1f);

                _createdObject.GetComponent<BoxCollider2D>().isTrigger = false;
                
                MapGenerator.Instance.BuildBridge(location, direction, bridgeObject.Location);
            }
            else {
                Destroy(_createdObject);
            }
            
            _cameraController.followMouse = false;
            _cameraController.target = NetworkManager.instance.LocalPlayer.transform;
        }
    }
}