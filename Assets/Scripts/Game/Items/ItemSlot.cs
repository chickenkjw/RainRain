using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Items
{
    /// <summary>
    /// Item 슬롯 오브젝트
    /// </summary>
    public class ItemSlot : MonoBehaviour, IDropHandler
    {
        /// <summary>
        /// 아이템을 드래그 후 떨어뜨릴 때 실행하는 함수
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrop(PointerEventData eventData) {
            InventoryItem inventoryItem;
            try {
                inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            }
            catch {
                return;
            }

            // 만약 슬롯에 아이템이 있지 않을 경우
            if (transform.childCount == 0) {
                inventoryItem.parentAfterDrag = transform;
            }
            // 슬롯에 이미 아이템이 있을 경우, 같은 아이템 타입인지 확인 후 합치기
            else {
                var slotItem = transform.GetChild(0).GetComponent<InventoryItem>();
                if (inventoryItem.item.type == slotItem.item.type) {
                    slotItem.count += inventoryItem.count;
                    
                    // 한 아이템을 들 수 있는 최대치까지 들었을 때
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