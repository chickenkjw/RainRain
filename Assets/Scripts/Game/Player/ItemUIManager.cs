using Game.Items;
using UnityEngine;

namespace Game.Player
{
    public class ItemUIManager : MonoBehaviour
    {
        #region EditorFields

        [SerializeField] 
        private PlayerManager player;

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

        public GameObject playerUI;

        #endregion

        #region Variables

        // UI가 열려있는가
        private bool _isOpeningInventory;
        
        #endregion

        private void Start() {
            _inventoryManager = InventoryManager.instance;
            
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
            
            playerUI = GameObject.FindGameObjectWithTag("PlayerUI");

            uiBackground = playerUI.transform.GetChild(0).gameObject;
            inventoryUI = playerUI.transform.GetChild(1).gameObject;
            boxContentsUI = playerUI.transform.GetChild(2).gameObject;
        }
        
        public bool OpenInventory(bool canInteractWithBox, GameObject[] pItems, GameObject[] bItems) {
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
                            bool result = _inventoryManager.AddItem(pItems[i].GetComponent<Item>(), i);
                            if (result) {
                                pItems[i] = player.noneItem;
                            }
                        }
                    }
                    
                    for(int i = 0; i < bItems.Length; i++) {
                        if (bItems[i] != null) {
                            bool result = _inventoryManager.AddItem(bItems[i].GetComponent<Item>(), i, true, false);
                            if (result) {
                                bItems[i] = player.noneItem;
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
                            bool result = _inventoryManager.AddItem(pItems[i].GetComponent<Item>(), i);
                            if (result) {
                                pItems[i] = player.noneItem;
                                pItems[i] = player.noneItem;
                            }
                        }
                    }
                }
            }
            else {
                _isOpeningInventory = false;
                
                // 인벤토리를 닫으면 그린 아이템을 모두 지우기
                var inventorySlots = inventoryUI.GetComponentsInChildren<Transform>();
                var boxSlots = boxContentsUI.GetComponentsInChildren<Transform>();
                var none = player.noneItem;
                GameObject[] items = { none, none, none, none };
                GameObject[] box = { none, none };
                int itemIndex = 0;
                int boxIndex = 0;
                
                for (int i = 1; i < inventorySlots.Length; i++) {
                    if (inventorySlots[i].transform.childCount > 0) {
                        var inventory = inventorySlots[i].transform.GetChild(0).GetComponent<InventoryItem>();
                        if (inventory == null) {
                            continue;
                        }

                        items[itemIndex] = inventory.item.gameObject;
                        itemIndex++;

                        Destroy(inventorySlots[i].transform.GetChild(0).gameObject);
                    }
                }
                
                player._playerItems = items;

                for (int i = 1; i < boxSlots.Length; i++) {
                    if (boxSlots[i].transform.childCount > 0) {
                        var inventory = boxSlots[i].transform.GetChild(0).GetComponent<InventoryItem>();
                        if (inventory == null) {
                            continue;
                        }

                        box[boxIndex] = inventory.item.gameObject;
                        boxIndex++;

                        Destroy(boxSlots[i].transform.GetChild(0).gameObject);
                    }
                }

                player._boxItems = box;
                
                uiBackground.SetActive(false);
                inventoryUI.SetActive(false);
                boxContentsUI.SetActive(false);
            }

            return _isOpeningInventory;
        }
    }
}