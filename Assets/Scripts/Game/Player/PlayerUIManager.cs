using System.Collections.Generic;
using System.Linq;
using Game.Items;
using UnityEngine;

namespace Game.Player
{
    public class PlayerUIManager : MonoBehaviour
    {
        #region EditorFields

        [Header("UI 오브젝트")]
        
        [SerializeField] 
        [Tooltip("플레이어의 인벤토리 UI")]
        private GameObject inventoryUI;

        [SerializeField] 
        [Tooltip("상자의 내용물 UI")]
        private GameObject boxContentsUI;

        [SerializeField] 
        [Tooltip("인벤토리가 켜질 때 주위 어두워지는거")]
        private GameObject uiBackground;

        #endregion
        
        #region Variables

        // 아이템을 그릴 때의 위치 변수 배열
        private GameObject[] _inventoryItemDrawPoint;
        private Transform[] _boxItemDrawPoint;

        // 아이템 생성 시 둘 부모 오브젝트
        private Transform _inventoryObjectsParent;
        private Transform _itemObjectsParent;
        
        // 아이템 매니저로부터 아이템을 가져옴
        private List<Item> _items;
        
        // UI 그릴 것들의 부모 오브젝트, 0->player, 1->box
        private Transform[] _drawObjectParents;
        
        #endregion

        private void Start() {
            SetVariables();
            
            inventoryUI.SetActive(false);
            boxContentsUI.SetActive(false);
            uiBackground.SetActive(false);
        }

        /// <summary>
        /// 변수 초기화 함수
        /// </summary>
        private void SetVariables() {
            _inventoryItemDrawPoint = new GameObject[4];
            _boxItemDrawPoint = new Transform[2];

            _items = GameObject.Find("ItemManager").GetComponent<ItemManager>().items;

            var drawParents = GameObject.FindGameObjectsWithTag("DrawObjectContainer");

            if(drawParents[0].name.Equals("PlayerDrawObjectParent")) {
                _drawObjectParents[0] = drawParents[0].transform;
                _drawObjectParents[1] = drawParents[1].transform;
            }
            else {
                _drawObjectParents[0] = drawParents[1].transform;
                _drawObjectParents[1] = drawParents[0].transform;
            }
            
            var playerUI = GameObject.FindGameObjectWithTag("PlayerUI");

            uiBackground = playerUI.transform.GetChild(0).gameObject;
            inventoryUI = playerUI.transform.GetChild(1).gameObject;
            boxContentsUI = playerUI.transform.GetChild(2).gameObject;
            
            for (int i = 0; i < 4; i++) {
                _inventoryItemDrawPoint[i] = inventoryUI.transform.GetChild(i).gameObject;
            }

            _boxItemDrawPoint[0] = boxContentsUI.transform.GetChild(0);
            _boxItemDrawPoint[1] = boxContentsUI.transform.GetChild(1);

            _inventoryObjectsParent = _drawObjectParents[0];
            _itemObjectsParent = _drawObjectParents[1];
        }
        
        public void OpenInventory(bool isOpeningInventory, bool canInteractWithBox, 
            ref Item[] pItems, ref Item[] bItems) {
            var rectTransform = inventoryUI.GetComponent<RectTransform>();
            
            if (!isOpeningInventory) {
                uiBackground.SetActive(true);

                // 박스와 인벤토리 동시 열기
                if (canInteractWithBox) {
                    rectTransform.anchoredPosition += Vector2.left * 105;
                    inventoryUI.SetActive(true);
                    boxContentsUI.SetActive(true);
                    DrawItemsOnInventory(ref pItems, ref bItems , true);
                }
                // 개인 인벤토리 열기
                else {
                    rectTransform.anchoredPosition = Vector2.zero;
                    inventoryUI.SetActive(true);
                    DrawItemsOnInventory(ref pItems, ref bItems);
                }
            }
            else {
                rectTransform.anchoredPosition = Vector2.zero;
                
                EraseItemsOnInventory(canInteractWithBox);

                uiBackground.SetActive(false);
                inventoryUI.SetActive(false);
                boxContentsUI.SetActive(false);
            }
        }
        
        /// <summary>
        /// inventory UI를 활성화할 때, 아이템을 그림
        /// </summary>
        /// <param name="isBoxInteraction"></param>
        private void DrawItemsOnInventory(ref Item[] playerItems, ref Item[] boxItems, bool isBoxInteraction = false) {
            for (int i = 0; i < 4; i++) {
                if(playerItems[i] != null) {
                    var items = playerItems;
                    Instantiate(_items.First(x => x.Equals(items[i])).gameObject, 
                        _inventoryItemDrawPoint[i].transform.position, Quaternion.identity, _inventoryObjectsParent);
                }
            }
            
            if (isBoxInteraction) {
                for (int i = 0; i < 2; i++) {
                    if (boxItems[i]  != null) {
                        var items = boxItems;
                        Instantiate(_items.First(x => x.Equals(items[i])).gameObject,
                            _boxItemDrawPoint[i].transform.position, Quaternion.identity, _itemObjectsParent);
                    }
                }
            }
        }

        /// <summary>
        /// inventory UI를 비활성화 시킬 때, 표시된 아이템을 지움
        /// </summary>
        /// <param name="isInteractWithBox"></param>
        private void EraseItemsOnInventory(bool isInteractWithBox = false) {
            var drawItems = _inventoryObjectsParent.GetComponentsInChildren<Transform>();

            for (int i = 1; i < drawItems.Length; i++) {
                Destroy(drawItems[i].gameObject);
            }

            if (isInteractWithBox) {
                var boxItems = _itemObjectsParent.GetComponentsInChildren<Transform>();

                for (int i = 1; i < boxItems.Length; i++) {
                    Destroy(boxItems[i].gameObject);
                }
            }
        }
    }
}