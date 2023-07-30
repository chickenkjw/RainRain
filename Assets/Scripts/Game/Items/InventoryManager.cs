using UnityEngine;

namespace Game.Items
{
    public class InventoryManager: MonoBehaviour
    {
        public ItemSlot[] itemSlots;
        public ItemSlot[] boxSlots;
        public GameObject inventoryItemPrefab;

        public bool AddItem(Item item, bool isBoxInteract = false) {
            foreach (var slot in isBoxInteract ? boxSlots : itemSlots) {
                InventoryItem itemSlot;
                if (slot.transform.childCount == 1) {
                    itemSlot = slot.transform.GetChild(0).GetComponent<InventoryItem>();
                }
                else {
                    break;
                }
                
                if (itemSlot != null && itemSlot.item.type == item.type && itemSlot.count < item.carryLimit && item.stackable) {
                    itemSlot.count++;
                    itemSlot.UpdateCountText();
                    return true;
                }
            }
            
            foreach (var slot in isBoxInteract ? boxSlots : itemSlots) {
                InventoryItem itemSlot = slot.GetComponent<InventoryItem>();
                if (itemSlot == null) {
                    SpawnNewItem(item, slot);
                    return true;
                }
            }

            return false;
        }

        private void SpawnNewItem(Item item, ItemSlot slot) {
            GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
            InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
            inventoryItem.InitializeItem(item);
        }
    }
}