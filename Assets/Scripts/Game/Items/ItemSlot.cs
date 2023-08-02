using Game.Fields;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Items
{
    /// <summary>
    /// Item 슬롯 오브젝트 (아이템 놓을 때 받는 역할)
    /// </summary>
    public class ItemSlot : MonoBehaviour, IDropHandler
    {
        public BoxItemIndex itemIndex;
        
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

            bool isTakeAll = true;

            // 만약 슬롯에 아이템이 있지 않을 경우
            if (transform.childCount == 0) {
                inventoryItem.parentAfterDrag = transform;
            }
            // 슬롯에 이미 아이템이 있을 경우, 같은 아이템 타입인지 확인 후 합치기
            else if(transform.childCount == 1){
                var slotItem = transform.GetChild(0).GetComponent<InventoryItem>();
                if (inventoryItem.item.type == slotItem.item.type) {
                    slotItem.item.count += inventoryItem.item.count;
                    
                    // 한 아이템을 들 수 있는 최대치까지 들었을 때
                    var limit = slotItem.item.carryLimit;
                    if (slotItem.item.count > limit) {
                        inventoryItem.item.count -= limit;
                        slotItem.item.count = limit;
                        
                        isTakeAll = false;

                        inventoryItem.UpdateCountText();
                    }
                    else {
                        print("옮긴 아이템 삭제");
                        Destroy(inventoryItem.gameObject);
                    }
                    
                    slotItem.UpdateCountText();
                }
            }
            else {
                return;
            }

            var manager = BoxContentsManager.Instance;
            bool result = manager.TakeItem((manager.boxDirection, manager.currentBoxItemIndex, isTakeAll));
        }        
    }
}