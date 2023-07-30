using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Items
{
    public class ItemSlot : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData) {
            InventoryItem inventoryItem;
            try {
                inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            }
            catch {
                return;
            }

            if (transform.childCount == 0) {
                inventoryItem.parentAfterDrag = transform;
            }
            else {
                var slotItem = transform.GetChild(0).GetComponent<InventoryItem>();
                if (inventoryItem.item.type == slotItem.item.type) {
                    slotItem.count += inventoryItem.count;
                    
                    var limit = slotItem.item.carryLimit;
                    if (slotItem.count > limit) {
                        inventoryItem.count -= limit;
                        slotItem.count = limit;
                    }
                    else {
                        Destroy(inventoryItem.gameObject);
                    }
                    
                    slotItem.UpdateCountText(); 
                }
            }
        }        
    }
}