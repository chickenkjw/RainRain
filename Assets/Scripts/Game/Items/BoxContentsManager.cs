using Game.Fields;
using UnityEngine;
using Photon.Pun;

namespace Game.Items
{
    public class BoxContentsManager : MonoBehaviour
    {
        #region Variables

        public (BoxContents, BoxContents)[][] BoxContentsArray;
                
        private MapGenerator _mapGenerator;
        
        private static BoxContentsManager _instance;

        public static BoxContentsManager Instance{
            get {
                if (_instance == null) {
                    return null;
                }
                
                return _instance;
            }
        }

        // 상자가 있는 건물의 x, y좌표 저장
        public Location BoxLocation;
        // 박스가 좌, 우 중 어디인지 저장
        public BoxDirection boxDirection;
        // 박스 슬롯이 위, 아래 중 어디에 있는 슬롯인지 저장
        public BoxItemIndex currentBoxItemIndex;

        [Tooltip("자신의 Photon View 파일")]
        public PhotonView PV;

        #endregion

        #region MonoBehaviour Callbacks

        void Awake() {
            if (_instance == null) {
                _instance = this;
                
                DontDestroyOnLoad(this.gameObject);
            }
            else {
                Destroy(this.gameObject);
            }
        }

        #endregion

        /// <summary>
        /// 박스들을 담은 배열 만들기
        /// </summary>
        public void SetBoxContents() {
            _mapGenerator = MapGenerator.Instance;
            
            BoxContentsArray = new (BoxContents, BoxContents)[_mapGenerator.buildingCount][];

            for (int i = 0; i < _mapGenerator.buildingCount; i++) {
                var height = _mapGenerator.BuildingArray[i].Length;
                BoxContentsArray[i] = new (BoxContents, BoxContents)[height];
                
                for (int j = 0; j < height; j++) {
                    BoxContentsArray[i][j] = (
                        _mapGenerator.BuildingArray[i][j].Object.transform.GetChild(1).GetChild(0)
                            .GetComponent<BoxContents>(),
                        _mapGenerator.BuildingArray[i][j].Object.transform.GetChild(1).GetChild(1)
                            .GetComponent<BoxContents>()
                    );
                }
            }
        }




        /// <summary>
        /// 아이템을 옮기는 함수
        /// </summary>
        /// <param name="movedItem">박스 방향(왼쪽, 오른쪽), 박스아이템위치(위, 아래), 다 옮기는지 여부</param>
        /// <returns>성공적으로 옮겼을 때 true 반환</returns>
        public bool TakeItem((BoxDirection, BoxItemIndex, bool) movedItem) {
            if (BoxLocation != null) {
                GameObject floorObj = _mapGenerator.BuildingArray[BoxLocation.X][BoxLocation.Y].Object;
                BoxContents boxContents = floorObj.transform.GetChild(1).GetChild((int)movedItem.Item1)
                    .GetComponent<BoxContents>();

                if (movedItem.Item3) {
                    if (movedItem.Item2 == BoxItemIndex.Up) {
                        boxContents.item1 = null;
                    }
                    else {
                        boxContents.item2 = null;
                    }
                }
                else {
                    if (movedItem.Item2 == BoxItemIndex.Up) {
                        boxContents.item1.GetComponent<Item>().count = 1;
                    }
                    else {
                        boxContents.item2.GetComponent<Item>().count = 1;
                    }
                }
            }

            PV.RPC(nameof(TakeItemRPC), RpcTarget.AllBuffered, movedItem);

            return true;
        }

        [PunRPC]
        public void TakeItemRPC((BoxDirection, BoxItemIndex, bool) movedItem)
        {
            Debug.Log($"아이템을 가져갔습니다! {movedItem.Item1}, {movedItem.Item2}");
        }
    }


    #region enums

    public enum BoxDirection
    {
        Left, Right, None
    }

    public enum BoxItemIndex
    {
        Up, Down, None
    }

    #endregion
    
}
