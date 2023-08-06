using System.Collections.Generic;
using System.Linq;
using Game.Items;
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
        
        [SerializeField] 
        [Tooltip("건물 개수")]
        public int buildingCount;
        
        [SerializeField] 
        [Tooltip("건물의 최대 층수 설정")]
        private int maxHeight;

        [SerializeField] 
        [Tooltip("건물의 최소 층수 설정")]
        private int minHeight;

        [SerializeField] 
        [Tooltip("이전 건물과의 층수 차이")]
        private int heightInterval;

        [HideInInspector] 
        [Tooltip("Environment 오브젝트. 건물의 부모 오브젝트")]
        public GameObject environmentObject;

        [SerializeField] 
        [Tooltip("부서진 다리 오브젝트")] 
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

        private List<GameObject> _items;

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
            
            _items = GameObject.Find("ItemManager").GetComponent<ItemManager>().items.ToList();

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

                currentHeight = prevHeight + randomHeightInterval;

                currentHeight = Mathf.Max(minHeight, currentHeight);
                currentHeight = Mathf.Min(maxHeight, currentHeight);

                BuildingArray[w] = new Floor[currentHeight];
                
                for (int h = 0; h < BuildingArray[w].Length; h++) {
                    int randomType = random.Next(maxValue: floorObjects.Count);

                    var floor = new Floor {
                        Object = floorObjects[randomType],
                        BuildPoint = buildPoint,
                        Location = new Location{ X = w, Y = h },
                    };

                    var floorObject = Instantiate(floor.Object, buildPoint, Quaternion.identity);
                    floorObject.transform.parent = environmentObject.transform.GetChild(0);
                    
                    var linkPoints = floorObject.transform.GetChild(3);
                    var leftBridgePoint = linkPoints.GetChild(0).GetComponent<Bridge>();
                    leftBridgePoint.location = new Location { X = w, Y = h };
                    var rightBridgePoint = linkPoints.GetChild(1).GetComponent<Bridge>();
                    rightBridgePoint.location = new Location { X = w, Y = h };

                    // 천장일 때 
                    if (h == BuildingArray[w].Length - 1) {
                        floorObject.transform.GetChild(0).gameObject.SetActive(false);
                        floorObject.transform.GetChild(5).gameObject.SetActive(true);
                        floorObject.transform.GetChild(3).GetChild(2).gameObject.SetActive(false);
                    }
                    
                    floor.Object = floorObject;
                    
                    BuildingArray[w][h] = floor;

                    SetBoxContents(floorObject, random, floor.Location);
                    
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
            
            GameObject.Find("BoxContentsManager").GetComponent<BoxContentsManager>().SetBoxContents();
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

            BuildingArray[x][y].Nodes[Direction.Right] = true;
            BuildingArray[x - 1][y].Nodes[Direction.Left] = true;
            
            // 다리 생성
            Vector3 startPoint = startFloor.transform.position;
            Vector3 endPoint = endFloor.transform.position;
            
            float distance = Vector2.Distance(startPoint, endPoint) / 29.5f;

            startPoint.x -= floorWidth * .75f;
            startPoint.y -=  floorHeight * .46f;
            var placedBridge = Instantiate(brokenBridge, startPoint, Quaternion.identity);

            placedBridge.GetComponent<BridgeObject>().ConnectedLoc = new Location { X = x, Y = y };
            placedBridge.transform.localScale = new Vector3(distance + 0.1f, .25f, 1f);
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
        /// <param name="location">x, y좌표</param>
        private void SetBoxContents(GameObject floorObj, Random random, Location location) {
            var boxes = floorObj.transform.GetChild(1);
            var box1 = boxes.GetChild(0).GetComponent<BoxContents>();
            var box2 = boxes.GetChild(1).GetComponent<BoxContents>();
            box1.Location = location;
            box2.Location = location;
            box1.boxDirection = BoxDirection.Left;
            box2.boxDirection = BoxDirection.Right;

            int firstBox = random.Next(maxValue: 100);
            int secondBox = random.Next(maxValue: 100);

            var boxParent = environmentObject.transform.GetChild(2);

            if (firstBox <= itemCreationRate) {
                var item1 = Instantiate(_items[firstBox % _itemTypeCount], box1.transform.position, Quaternion.identity);
                item1.transform.parent = boxParent;
                item1.GetComponent<Item>().count = 1;
                box1.item1 = item1;

                int nextItem = random.Next(maxValue: 100);

                if (nextItem <= itemCreationRate * secondItemCreationRate) {
                    var item2 = Instantiate(_items[nextItem % _itemTypeCount], box1.transform.position, Quaternion.identity);
                    item2.transform.parent = boxParent;
                    item2.GetComponent<Item>().count = 1;
                    box1.item2 = item2;
                }
            }
            
            if (secondBox <= itemCreationRate) {
                var item1 = Instantiate(_items[secondBox % _itemTypeCount], box2.transform.position, Quaternion.identity);
                item1.transform.parent = boxParent;
                item1.GetComponent<Item>().count = 1;
                box2.item1 = item1;
                
                int nextItem = random.Next(maxValue: 100);

                if (nextItem <= itemCreationRate * secondItemCreationRate) {
                    var item2 =  Instantiate(_items[nextItem % _itemTypeCount], box2.transform.position, Quaternion.identity);
                    item2.transform.parent = boxParent;
                    item2.GetComponent<Item>().count = 1;
                    box2.item2 = item2;
                }
            }
        }

        public void RemoveWalls(Location start, Direction startDir, Location end) {
            var startWalls = BuildingArray[start.X][start.Y].Object.transform.GetChild(2);
            var endWalls = BuildingArray[end.X][end.Y].Object.transform.GetChild(2);
            int wallDir = startDir == Direction.Left ? 1 : 0;

            startWalls.GetChild(wallDir).gameObject.SetActive(false);
            endWalls.GetChild((wallDir + 1) % 2).gameObject.SetActive(false);
        }

        public void EnablePoints(Location start, Direction dir, Location end) {
            int startIndex = dir == Direction.Left ? 1 : 0;
            var startPoint = BuildingArray[start.X][start.Y].Object.transform.GetChild(3).GetChild(startIndex);
            var endPoint = BuildingArray[end.X][end.Y].Object.transform.GetChild(3).GetChild((startIndex + 1) % 2);
            startPoint.gameObject.SetActive(false);
            endPoint.gameObject.SetActive(false);
        }

        public (Transform, Location) GetDestinationFloor(Location startPoint, Direction direction, int limit) {
            int i = 1;
            while (i <= limit) {
                try {
                    int ii = i * (direction == Direction.Left ? 1 : -1);
                    var loc = BuildingArray[startPoint.X + ii][startPoint.Y].Location;
                    var floor = BuildingArray[startPoint.X + ii][startPoint.Y].Object.transform;
                    return (floor, loc);
                }
                catch {
                   i++; 
                }
            }

            return (null, null);
        }
    }
}
