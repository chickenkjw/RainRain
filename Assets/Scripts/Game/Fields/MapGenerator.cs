using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Game.Fields
{
    /// <summary>
    /// Game의 맵을 생성하는 클래스
    /// </summary>
    public class MapGenerator : MonoBehaviour
    {
        #region EditorParameters
        
        [FormerlySerializedAs("floors")]
        [Header("게임 오브젝트")] 
    
        [SerializeField] 
        [Tooltip("건물 한 층 오브젝트")]
        private List<GameObject> floorObjects;

        [Header("생성 파라미터")] 
        [SerializeField] [Tooltip("건물 개수")]
        private int buildingCount;
        
        [SerializeField] 
        [Tooltip("건물의 최대 높이 설정")]
        private int maxHeight;

        [SerializeField] 
        [Tooltip("건물의 최소 높이 설정")]
        private int minHeight;

        [SerializeField] 
        [Tooltip("이전 건물과의 높이 차이")]
        private int heightInterval;

        [SerializeField] 
        [Tooltip("Environment 오브젝트. 건물의 부모 오브젝트")]
        private GameObject environmentObject;

        [SerializeField] 
        [Tooltip("다리 오브젝트")] 
        private GameObject bridge;

        [SerializeField] 
        [Tooltip("다리 생성 확률. 입력범위: 0~100")]
        private int bridgeCreationRate;
        
        [SerializeField] 
        [Tooltip("계단 생성 확률. 입력범위: 0~100")]
        private int stairCreationRate;
        
        #endregion

        #region Variables

        private static MapGenerator _instance;
        
        private Floor[][] _buildingArray;

        private float _floorHeight;
        private float _floorWidth;
        
        #endregion
        
        public static MapGenerator Instance
        {
            get {
                if (null == _instance) {
                    return null;
                }
                
                return _instance;
            }
        }

        void Awake() {
            if (_instance == null) {
                _instance = this;
                
                DontDestroyOnLoad(this.gameObject);
            }
            else {
                Destroy(this.gameObject);
            }
        }

        private void Start() {
            _floorHeight = floorObjects[0].GetComponent<SpriteRenderer>().bounds.size.y;
            _floorWidth = floorObjects[0].GetComponent<SpriteRenderer>().bounds.size.x;
        }

        /// <summary>   
        /// 맵 생성 함수
        /// </summary>
        public void GenerateMap() {
            _buildingArray = new Floor[buildingCount][];
            Vector2 buildPoint = Vector2.zero;
            
            Random random = new();
            int randomHeight = (int)(maxHeight * 0.6f);
            int currentHeight = randomHeight;
            int prevHeight = currentHeight;

            for (int w = 0; w < buildingCount; w++) {
                int randomHeightInterval = random.Next(minValue: -heightInterval, maxValue: heightInterval + 1);

                currentHeight = prevHeight + randomHeightInterval;

                currentHeight = Mathf.Max(minHeight, currentHeight);
                currentHeight = Mathf.Min(maxHeight, currentHeight);

                _buildingArray[w] = new Floor[currentHeight];
                
                for (int h = 0; h < _buildingArray[w].Length; h++) {
                    int randomType = random.Next(maxValue: floorObjects.Count);

                    var floor = new Floor {
                        Object = floorObjects[randomType],
                        BuildPoint = buildPoint,
                        Location = new Location{ X = w, Y = h }
                    };
                    _buildingArray[w][h] = floor;
                    
                    var floorObject = Instantiate(floor.Object, buildPoint, Quaternion.identity);
                    floorObject.transform.parent = environmentObject.transform.GetChild(0);

                    buildPoint.y += _floorHeight;

                    // 계단 없애기
                    if (random.Next(maxValue: 100) > stairCreationRate) {
                        RemoveStair(floorObject.transform);
                    }
                    
                    // 다리 놓기
                    if (random.Next(maxValue: 100) <= bridgeCreationRate) {
                        PlaceBridge(w, h);
                    }
                }
                
                buildPoint.y = 0;
                buildPoint.x += _floorWidth * 1.5f;

                prevHeight = currentHeight + (currentHeight == minHeight ? 3 : currentHeight == maxHeight ? -3 : 0);
            }
        }

        private void RemoveStair(Transform floor) {
            floor.GetChild(0).gameObject.SetActive(false);
            floor.GetChild(3).GetChild(2).gameObject.SetActive(false);
            floor.GetChild(3).GetChild(3).gameObject.SetActive(false);
        }

        private void PlaceBridge(int x, int y) {
            // 벽 콜라이더 없애기
            Floor startFloor = _buildingArray[x][y];
            Floor endFloor;

            try {
                endFloor = _buildingArray[x - 1][y];
            }
            catch {
                return;
            }

            startFloor.Object.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
            endFloor.Object.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
            
            // 다리 생성
            Vector3 startPoint = startFloor.BuildPoint + startFloor.Object.transform.GetChild(3).GetChild(0).position;
            Vector3 endPoint = endFloor.BuildPoint + endFloor.Object.transform.GetChild(3).GetChild(1).position;

            float distance = Vector2.Distance(startPoint, endPoint) * 0.1f;

            startPoint.x -= _floorWidth * .25f;
            var placedBridge = Instantiate(bridge, startPoint, Quaternion.identity);
            
            placedBridge.transform.localScale = new Vector3(distance, .3f, 1f);

            placedBridge.transform.parent = environmentObject.transform.GetChild(1);
        }
    }
}
