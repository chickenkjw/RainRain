using System.Collections.Generic;
using System.Linq;
using Game.Camera;
using UnityEngine;
using Cam = UnityEngine.Camera;
using PlayerManager = Game.Player.PlayerManager;

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

        public bool connectedBrokenBridge;

        [SerializeField]
        private List<GameObject> bridges;
        
        private void Start() {
            _bridgeObject = direction == Direction.Left ? bridges[1] : bridges[0];
            connectedBrokenBridge = MapGenerator.Instance.BuildingArray[location.X][location.Y].ConnectBrokenBridge;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.transform.CompareTag("Player")) {
                print("이제부터 여기로 계산됩니다");
                var player = other.GetComponent<PlayerManager>();
                player.canBuildBridge = true;
                player.bridge = this;
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.transform.CompareTag("Player")) {
                var player = other.GetComponent<PlayerManager>();
                player.canBuildBridge = false;
                player.bridge = null;
            }
        }

        public bool BuildBridge() {
            var destination = MapGenerator.Instance.GetDestinationFloor(location, direction, 2);

            if (destination == null) {
                return false;
            }
            
            // 부서진 다리 공사
            if (connectedBrokenBridge) {
                var brokenBridges = GameObject.FindGameObjectsWithTag("Bridge");
                var brokenBridge = brokenBridges
                    .First(x => (x.GetComponent<BridgeObject>().ConnectedLoc.Equals(location) || 
                                            x.GetComponent<BridgeObject>().inverseLoc.Equals(location)));
                Destroy(brokenBridge);
            }
            
            var createdBridge = Instantiate(_bridgeObject, transform.position, Quaternion.identity);

            float distance = Vector2.Distance(transform.position, destination.position) / 9.8f;
            
            createdBridge.transform.localScale = new Vector3(distance, .3f, 1f);
            
            var loc = location;
            loc.X += direction == Direction.Right ? -1 : 1;
            MapGenerator.Instance.RemoveWalls(location, direction, loc);
            MapGenerator.Instance.EnablePoints(location, direction, loc);
            
            return true;
        }
    }
}