using UnityEngine;

namespace Game.Items
{
    /// <summary>
    /// 아이템 인벤토리 매니저
    /// </summary>
    public class InventoryManager: MonoBehaviour
    {
        // 플레이어의 아이템 슬롯 (4개)
        public ItemSlot[] itemSlots;
        // 박스의 아이템 슬롯 (2개)
        public ItemSlot[] boxSlots;
        [Tooltip("인벤토리 슬롯 프리팹(InventoryItem이 달린 오브젝트)")] 
        public GameObject inventoryItemPrefab;

        public static InventoryManager instance;

        void Awake()
        {
            // SoundManager 인스턴스가 이미 있는지 확인, 이 상태로 설정
            if (instance == null)
                instance = this;

            // 인스턴스가 이미 있는 경우 오브젝트 제거
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 아이템 배열을 받아와 그리는 함수
        /// </summary>
        /// <param name="item">그릴 아이템</param>
        /// <param name="isBoxInteract">박스를 연 상태인가</param>
        /// <param name="isSameIndex">같은 아이템이더라도 다르게 배치할 수 있도록 하는 장치</param>
        /// <returns></returns>
        public bool AddItem(Item item, int index, bool isBoxInteract = false, bool isSameIndex = true) {
            if (item.type == ItemType.None) {
                return false;
            }
            
            var itemArray = isBoxInteract ? boxSlots : itemSlots;
            
            // 아이템 슬롯이 차있는데, 만약 같은 타입의 아이템이라면 쌓기
            if (isSameIndex) {
                InventoryItem itemSlot = itemArray[index].transform.childCount == 1 ? 
                    itemArray[index].transform.GetChild(0).GetComponent<InventoryItem>() 
                    : null;

                if (itemSlot != null && itemSlot.item.type == item.type && itemSlot.item.count < item.carryLimit &&
                    item.stackable) {
                    itemSlot.item.count++;
                    itemSlot.UpdateCountText();
                    return true;
                }
            }

            // 아이템 슬롯이 비어있다면 아이템을 그리기
            var slot = (isBoxInteract ? boxSlots : itemSlots)[index];
            InventoryItem itemSlot2 = slot.transform.childCount > 0 ?
                    slot.transform.GetChild(0).GetComponent<InventoryItem>()
                    : null;

            if (itemSlot2 == null) {
                SpawnNewItem(item, slot);
                return true;
            }

            // 만약 아이템을 슬롯에 추가할 수 없는 경우, false 반환
            return false;
        }

        // 드래그 가능한 아이템을 그리는 함수
        private void SpawnNewItem(Item item, ItemSlot slot) {
            GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
            InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
            inventoryItem.InitializeItem(item, 1);
        }
    }
}