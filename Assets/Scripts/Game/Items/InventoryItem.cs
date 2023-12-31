﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Items
{
    /// <summary>
    /// 아이템 슬롯에 배치할(그릴) 아이템 
    /// </summary>
    public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // 그릴 이미지 (스프라이트는 item에서 가져올 것임)
        public Image image;
        // 개수 텍스트의 밑바탕
        public Image darkBackground;
        // 개수 텍스트
        public Text countText;

        // 아이템 출발지
        private BoxItemIndex _boxItemIndex;

        public int count;

        // 슬롯에 들어갈 아이템
        [HideInInspector] public Item item;
        // 아이템 위치 배정을 위한 부모 오브젝트 위치
        [HideInInspector] public Transform parentAfterDrag;

        /// <summary>
        /// 아이템 그리기
        /// </summary>
        /// <param name="newItem">배정할 아이템</param>
        /// <param name="count"></param>
        public void InitializeItem(Item newItem, int count) {
            item = newItem;
            image.sprite = newItem.image;
            this.count = count;
            UpdateCountText();

            if (transform.parent.name.Equals("Item1")) {
                _boxItemIndex = BoxItemIndex.Up;
            }
            else if (transform.parent.name.Equals("Item2")) {
                _boxItemIndex = BoxItemIndex.Down;
            }
            else {
                _boxItemIndex = BoxItemIndex.None;
            }
        }

        public void UpdateCountText() {
            countText.text = item.count.ToString();
            countText.gameObject.SetActive(item.count > 0);
            darkBackground.gameObject.SetActive(item.count > 0);
        }
        
        // 아이템 드래그 시작
        public void OnBeginDrag(PointerEventData eventData) {
            image.raycastTarget = false;
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            
            BoxContentsManager.Instance.currentBoxItemIndex = _boxItemIndex;
        }

        // 드래그 중, 아이템이 커서를 따라오도록 위치 변경
        public void OnDrag(PointerEventData eventData) {
            transform.position = Input.mousePosition;
        }

        // 드래그가 끝났을 때, 위치 슬롯 중앙으로 자동 정렬
        // 만약 놓은 곳이 슬롯이 아니면 제자리로
        public void OnEndDrag(PointerEventData eventData) {
            image.raycastTarget = true;
            transform.SetParent(parentAfterDrag);
            transform.position = parentAfterDrag.position;
        }
    }
}