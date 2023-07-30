using System.Collections.Generic;
using Game.Items;
using UnityEngine;

namespace Game.Player
{
    public class ItemUIManager : MonoBehaviour
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

        private InventoryManager _inventoryManager;

        #endregion
        
        #region Variables

        // 아이템을 그릴 때의 위치 변수 배열
        private GameObject[] _inventoryItemDrawPoint;
        private Transform[] _boxItemDrawPoint;

        // 아이템 매니저로부터 아이템을 가져옴
        private List<Item> _items;

        // UI가 열려있는가
        private bool _isOpeningInventory;
        
        #endregion

        private void Start() {
            _inventoryManager = GameObject.Find("ItemManager").GetComponent<InventoryManager>();
            
            SetVariables();
            
            inventoryUI.SetActive(false);
            boxContentsUI.SetActive(false);
            uiBackground.SetActive(false);
        }

        /// <summary>
        /// 변수 초기화 함수
        /// </summary>
        private void SetVariables() {
            _isOpeningInventory = false;
            
            _inventoryItemDrawPoint = new GameObject[4];
            _boxItemDrawPoint = new Transform[2];

            _items = GameObject.Find("ItemManager").GetComponent<ItemManager>().items;
            
            var playerUI = GameObject.FindGameObjectWithTag("PlayerUI");

            uiBackground = playerUI.transform.GetChild(0).gameObject;
            inventoryUI = playerUI.transform.GetChild(1).gameObject;
            boxContentsUI = playerUI.transform.GetChild(2).gameObject;
            
            /*for (int i = 0; i < 4; i++) {
                var drawPanel = inventoryUI.transform.GetChild(i);
                _inventoryItemDrawPoint[i] = drawPanel.GetChild(2).gameObject;
            }

            for (int i = 0; i < 2; i++) {
                var drawPanel = boxContentsUI.transform.GetChild(i);
                _boxItemDrawPoint[i] = drawPanel.GetChild(2);
            }*/
        }
        
        public bool OpenInventory(bool canInteractWithBox, ref Item[] pItems, ref Item[] bItems) {
            if (!_isOpeningInventory) {
                _isOpeningInventory = true;
                
                uiBackground.SetActive(true);
                
                var rectTransform = inventoryUI.GetComponent<RectTransform>();

                // 박스와 인벤토리 동시 열기
                if (canInteractWithBox) {
                    rectTransform.anchoredPosition = Vector2.left * 105;
                    inventoryUI.SetActive(true);
                    boxContentsUI.SetActive(true);
                    
                    for(int i = 0; i < pItems.Length; i++){
                        if (pItems[i] != null) {
                            bool result = _inventoryManager.AddItem(pItems[i]);
                            if (result) {
                                pItems[i] = null;
                            }
                        }
                    }

                    for(int i = 0; i < bItems.Length; i++) {
                        if (bItems[i] != null) {
                            bool result = _inventoryManager.AddItem(bItems[i], true);
                            if (result) {
                                bItems[i] = null;
                            }
                        }
                    }
                }
                // 개인 인벤토리 열기
                else {
                    rectTransform.anchoredPosition = Vector2.zero;
                    inventoryUI.SetActive(true);
                    
                    for (int i = 0; i < pItems.Length; i++) {
                        if (pItems[i] != null) {
                            bool result = _inventoryManager.AddItem(pItems[i]);
                            if (result) {
                                pItems[i] = null;
                                pItems[i] = null;
                            }
                        }
                    }
                }
            }
            else {
                _isOpeningInventory = false;
                
                uiBackground.SetActive(false);
                inventoryUI.SetActive(false);
                boxContentsUI.SetActive(false);
            }

            return _isOpeningInventory;
        }

        /// <summary>
        /// 사용하지 않음
        /// </summary>
        /// <param name="playerItems">playerInventory에 그려질 아이템</param>
        /// <param name="boxItems">boxContents에 그려질 아이템</param>
        /// <param name="isBoxInteraction">박스와 상호작용 중인가?</param>
        private void DrawItemsOnInventory(ref Item[] playerItems, ref Item[] boxItems, bool isBoxInteraction = false) {
            for (int i = 0; i < 4; i++) {
                if(playerItems[i] != null) {
                    var items = playerItems;
                    
                }
            }
            
            if (isBoxInteraction) {
                for (int i = 0; i < 2; i++) {
                    if (boxItems[i] != null) {
                        var items = boxItems;
                        
                    }
                }
            }
        }

        /// <summary>
        /// 사용하지 않음
        /// </summary>
        /// <param name="isInteractWithBox"></param>
        private void EraseItemsOnInventory(bool isInteractWithBox = false) {
            var drawItems = inventoryUI.GetComponentsInChildren<Transform>();

            foreach (var drawItem in drawItems) {
                if (drawItem.Equals(drawItems[0])) {
                    continue;
                }
                if (drawItem.childCount >= 4) {
                    Destroy(drawItem.GetChild(3).gameObject);
                }
            }

            if (isInteractWithBox) {
                var boxItems = boxContentsUI.GetComponentsInChildren<Transform>();

                foreach (var boxItem in boxItems) {
                    if(boxItem.childCount >= 4) {
                        Destroy(boxItem.GetChild(3).gameObject);
                    }
                }
            }
        }
    }
}