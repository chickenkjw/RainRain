using UnityEngine;

namespace Game.Items
{
    [System.Serializable]
    public class Item : MonoBehaviour
    {
        public GameObject itemObject;
        public int count;
        public int carryLimit;
        public ItemType type;
    }
}