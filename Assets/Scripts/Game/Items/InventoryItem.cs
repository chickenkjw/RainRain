using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Items
{
    public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Image image;
        public Image darkBackground;
        public Text countText;

        [HideInInspector] public int count = 1;
        [HideInInspector] public Item item;
        [HideInInspector] public Transform parentAfterDrag;

        public void InitializeItem(Item newItem) {
            item = newItem;
            image.sprite = newItem.image;
            UpdateCountText();
        }

        public void UpdateCountText() {
            countText.text = count.ToString();
            countText.gameObject.SetActive(count > 0);
            darkBackground.gameObject.SetActive(count > 0);
        }
        
        public void OnBeginDrag(PointerEventData eventData) {
            image.raycastTarget = false;
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
        }

        public void OnDrag(PointerEventData eventData) {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData) {
            image.raycastTarget = true;
            print($"{parentAfterDrag.name}");
            transform.SetParent(parentAfterDrag);
            transform.position = parentAfterDrag.position;
        }
    }
}