using UnityEngine;

namespace Game.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object Assets/Item")]
    public class Item : ScriptableObject
    {
        public ItemType type;
        public bool stackable;
        public Sprite image;
        public int carryLimit;
    }
}