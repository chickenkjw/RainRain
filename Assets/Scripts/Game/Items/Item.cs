using UnityEngine;

namespace Game.Items
{
    /// <summary>
    /// 아이템의 Scriptable Object
    /// </summary>
    // [CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object Assets/Item")]
    public class Item : MonoBehaviour
    {
        // 아이템의 타입
        public ItemType type;
        // 쌓을 수 있는 아이템인가? (만약 후에 도구 등을 추가할 수 있으므로)
        public bool stackable;
        // 아이템의 sprite
        public Sprite image;
        // 아이템을 한 슬롯에 최대로 담을 수 있는 개수
        public int carryLimit;
        // 아이템의 개수
        public int count;
    }
}