using System.Collections.Generic;
using Game.Items;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Game.Fields
{    /// <summary>
     /// 건물과 다리를 생성하는 데 필요한 시드 객체
     /// </summary>
    public class Building
    {
        public Floor[][] BuildingArray;
        public Vector2 BuildPoint;
        public int BuildingCount;
        public float FloorWidth;
        public float FloorHeight;
        public float MaxHeight;
        public float MinHeight;
        public List<GameObject> FloorObjects;
        public GameObject EnvironmentObject;
        public int HeightInterval;
        public int BridgeCreationRate;

        public Building(int buildingCount, float floorWidth, float floorHeight, float maxHeight, float minHeight, List<GameObject> floorObjects, GameObject environmentObject, int heightInterval, int bridgeCreationRate)
        {
            BuildingCount = buildingCount;
            FloorWidth = floorWidth;
            FloorHeight = floorHeight;
            MaxHeight = maxHeight;
            MinHeight = minHeight;
            FloorObjects = floorObjects;
            EnvironmentObject = environmentObject;
            HeightInterval = heightInterval;
            BridgeCreationRate = bridgeCreationRate;
            BuildingArray = new Floor[buildingCount][];
            BuildPoint = Vector2.zero - Vector2.up * floorHeight;
        }

    }


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
        
        [SerializeField] 
        [Tooltip("건물 개수")]
        private int buildingCount;
        
        [SerializeField] 
        [Tooltip("건물의 최대 층수 설정")]
        private int maxHeight;

        [SerializeField] 
        [Tooltip("건물의 최소 층수 설정")]
        private int minHeight;

        [SerializeField] 
        [Tooltip("이전 건물과의 층수 차이")]
        private int heightInterval;

        [SerializeField] 
        [Tooltip("Environment 오브젝트. 건물의 부모 오브젝트")]
        private GameObject environmentObject;

        [SerializeField] 
        [Tooltip("다리 오브젝트")] 
        private GameObject brokenBridge;

        [SerializeField] 
        [Range(0, 100)]
        [Tooltip("다리 생성 확률. 입력범위: 0~100")]
        private int bridgeCreationRate;

        [SerializeField] 
        [Range(0, 100)]
        [Tooltip("상자에 아이템이 있을 확률. 입력범위: 0~100")]
        private int itemCreationRate;

        [SerializeField] 
        [Range(0, 1)]
        [Tooltip("itemCreationRate에 이 값을 곱한 값이 두번째 아이템 확률임. 입력범위: 0~0.5")]
        private float secondItemCreationRate;

        #endregion

        #region Variables

        private static MapGenerator _instance;

        private List<Item> _items;

        [HideInInspector]
        public Floor[][] BuildingArray;

        private int _itemTypeCount;

        [HideInInspector]
        public float floorHeight;
        [HideInInspector]
        public float floorWidth;

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
            floorWidth = floorObjects[0].GetComponent<SpriteRenderer>().bounds.size.x;

            _items = GameObject.Find("ItemManager").GetComponent<ItemManager>().items;

            _itemTypeCount = _items.Count;
        }

        /// <summary>   
        /// 맵 생성 함수
        /// </summary>
        public void GenerateMap(int seed) {
            BuildingArray = new Floor[buildingCount][];
            Vector2 buildPoint = Vector2.zero - Vector2.up * floorHeight;

            Random random = new Random(seed);
            int randomHeight = (int)(maxHeight * 0.6f);
            int currentHeight = randomHeight;
            int prevHeight = currentHeight;

            for (int i = 0; i < buildingCount; i++) {
                var randomType = random.Next(maxValue: floorObjects.Count);
                var createObj = Instantiate(floorObjects[randomType], buildPoint, Quaternion.identity);
                createObj.transform.parent = environmentObject.transform;
                buildPoint.x += floorWidth * 1.5f;
            }
            
            buildPoint = Vector2.zero;

            for (int w = 0; w < buildingCount; w++) {
                int randomHeightInterval = random.Next(minValue: -heightInterval, maxValue: heightInterval + 1);
                Debug.LogFormat("randomHeight Interval : {0}", randomHeightInterval);

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
                    
                    SetBoxContents(floorObject, random);
                    
                    buildPoint.y += floorHeight;
                    
                    // 다리 놓기
                    if (w != 0 && random.Next(maxValue: 100) <= bridgeCreationRate) {
                        PlaceBrokenBridge(w, h);
                    }
                }
                
                buildPoint.y = 0;
                buildPoint.x += floorWidth * 1.5f;

                prevHeight = currentHeight + (currentHeight == minHeight ? 3 : currentHeight == maxHeight ? -3 : 0);
            }
        }

        /// <summary>
        /// 부서진 다리 생성하기
        /// </summary>
        /// <param name="x">가로 인덱스</param>
        /// <param name="y">세로 인덱스(층)</param>
        private void PlaceBrokenBridge(int x, int y) {
            // 벽 콜라이더 없애기
            GameObject startFloor = environmentObject.transform.GetChild(0).GetChild(GetCurrentBuildingIndex(x, y))
                .gameObject;

            try {
                if(BuildingArray[x - 1][y] == null) return;
            }
            catch {
                return;
            }
            
            GameObject endFloor = environmentObject.transform.GetChild(0)
                .GetChild(GetCurrentBuildingIndex(x - 1, y)).gameObject;
            
            startFloor.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
            endFloor.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
            
            // 다리 생성
            Vector3 startPoint = startFloor.transform.position;
            Vector3 endPoint = endFloor.transform.position;
            
            float distance = Vector2.Distance(startPoint, endPoint) / 29.5f;

            startPoint.x -= floorWidth * .75f;
            startPoint.y -=  floorHeight * .46f;
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

        /// <summary>
        /// floor object 안의 box 아이템 세팅
        /// </summary>
        /// <param name="floorObj">box object</param>
        /// <param name="random">random engine</param>
        private void SetBoxContents(GameObject floorObj, Random random) {
            var boxes = floorObj.transform.GetChild(1);
            var box1 = boxes.GetChild(0).GetComponent<BoxContents>();
            var box2 = boxes.GetChild(1).GetComponent<BoxContents>();

            int firstBox = random.Next(maxValue: 100);
            int secondBox = random.Next(maxValue: 100);

            if (firstBox <= itemCreationRate) {
                var item1 = _items[firstBox % _itemTypeCount];
                box1.item1 = item1;

                int nextItem = random.Next(maxValue: 100);

                if (nextItem <= itemCreationRate * secondItemCreationRate) {
                    var item2 = _items[nextItem % _itemTypeCount];
                    box1.item2 = item2;
                }
            }
            
            if (secondBox <= itemCreationRate) {
                var item1 = _items[secondBox % _itemTypeCount];
                box2.item1 = item1;
                
                int nextItem = random.Next(maxValue: 100);

                if (nextItem <= itemCreationRate * secondItemCreationRate) {
                    var item2 = _items[nextItem % _itemTypeCount];
                    box2.item2 = item2;
                }
            }
        }
    }
}
