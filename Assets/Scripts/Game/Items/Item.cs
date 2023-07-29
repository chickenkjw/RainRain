using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Items
{
    [System.Serializable]
    public class Item : MonoBehaviour
    {
        public int count;
        public ItemType type;
        
        [SerializeField]
        protected int carryLimit;
    }
}