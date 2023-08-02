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

        /// <summary>
        /// 아이템 배열을 받아와 그리는 함수
        /// </summary>
        /// <param name="item">그릴 아이템</param>
        /// <param name="isBoxInteract">박스를 연 상태인가</param>
        /// <returns></returns>
        public bool AddItem(Item item, bool isBoxInteract = false) {
            // 아이템 슬롯이 차있는데, 만약 같은 타입의 아이템이라면 쌓기
            foreach (var slot in isBoxInteract ? boxSlots : itemSlots) {
                InventoryItem itemSlot;
                if (slot.transform.childCount == 1) {
                    itemSlot = slot.transform.GetChild(0).GetComponent<InventoryItem>();
                }
                else {
                    break;
                }
                
                if (itemSlot != null && itemSlot.item.type == item.type && itemSlot.item.count < item.carryLimit && item.stackable) {
                    itemSlot.item.count++;
                    itemSlot.UpdateCountText();
                    return true;
                }
            }
            
            // 아이템 슬롯이 비어있다면 아이템을 그리기
            foreach (var slot in isBoxInteract ? boxSlots : itemSlots) {
                InventoryItem itemSlot = slot.GetComponent<InventoryItem>();
                if (itemSlot == null) {
                    SpawnNewItem(item, slot);
                    return true;
                }
            }

            // 만약 아이템을 슬롯에 추가할 수 없는 경우, false 반환
            return false;
        }

        // 드래그 가능한 아이템을 그리는 함수
        private void SpawnNewItem(Item item, ItemSlot slot) {
            GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
            InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
            inventoryItem.InitializeItem(item);
        }
    }
}