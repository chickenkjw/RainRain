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
        private GameObject brokenBridge;

        [SerializeField] 
        [Tooltip("다리 생성 확률. 입력범위: 0~100")]
        private int bridgeCreationRate;

        #endregion

        #region Variables

        private static MapGenerator _instance;
        
        public Floor[][] BuildingArray;

        [HideInInspector]
        public float floorHeight;
        private float _floorWidth;

        #endregion
        
        public static MapGenerator Instance
        {
            get {
                if (_instance == null) {
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
            
            floorHeight = floorObjects[0].GetComponent<SpriteRenderer>().bounds.size.y;
            _floorWidth = floorObjects[0].GetComponent<SpriteRenderer>().bounds.size.x;
        }

        /// <summary>   
        /// 맵 생성 함수
        /// </summary>
        public void GenerateMap() {
            BuildingArray = new Floor[buildingCount][];
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

                BuildingArray[w] = new Floor[currentHeight];
                
                for (int h = 0; h < BuildingArray[w].Length; h++) {
                    int randomType = random.Next(maxValue: floorObjects.Count);

                    var floor = new Floor {
                        Object = floorObjects[randomType],
                        BuildPoint = buildPoint,
                        Location = new Location{ X = w, Y = h }
                    };
                    BuildingArray[w][h] = floor;
                    
                    var floorObject = Instantiate(floor.Object, buildPoint, Quaternion.identity);
                    floorObject.transform.parent = environmentObject.transform.GetChild(0);
                    
                    buildPoint.y += floorHeight;
                    
                    // 다리 놓기
                    if (w != 0 && random.Next(maxValue: 100) <= bridgeCreationRate) {
                        PlaceBrokenBridge(w, h);
                    }
                }
                
                buildPoint.y = 0;
                buildPoint.x += _floorWidth * 1.5f;

                prevHeight = currentHeight + (currentHeight == minHeight ? 3 : currentHeight == maxHeight ? -3 : 0);
            }
        }

        private void PlaceBrokenBridge(int x, int y) {
            // 벽 콜라이더 없애기
            GameObject startFloor = environmentObject.transform.GetChild(0).GetChild(GetCurrentBuildingIndex(x, y))
                .gameObject;

            try {
                var temp = BuildingArray[x - 1][y];
            }
            catch {
                return;
            }
            
            GameObject endFloor = environmentObject.transform.GetChild(0).GetChild(GetCurrentBuildingIndex(x - 1, y))
                .gameObject;
            
            startFloor.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
            endFloor.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
            
            // 다리 생성
            Vector3 startPoint = startFloor.transform.position;
            Vector3 endPoint = endFloor.transform.position;
            
            float distance = Vector2.Distance(startPoint, endPoint) / 29.5f;

            startPoint.x -= _floorWidth * .75f;
            startPoint.y -=  floorHeight * .48f;
            var placedBridge = Instantiate(brokenBridge, startPoint, Quaternion.identity);
            
            placedBridge.transform.localScale = new Vector3(distance, .25f, 1f);

            placedBridge.transform.parent = environmentObject.transform.GetChild(1);
        }

        /// <summary>
        /// EnvironmentObject로부터 GameObject를 가져오기 위해 Child Index를 구하는 함수
        /// </summary>
        /// <param name="x">x번(가로)째</param>
        /// <param name="y">x번째 y층 건물</param>
        /// <returns>몇 번째 Index인지 반환</returns>
        private int GetCurrentBuildingIndex(int x, int y) {
            int sum = 0;

            for (int i = 0; i < x; i++) {
                sum += BuildingArray[i].Length;
            }

            sum += y;

            return sum;
        }
    }
}
