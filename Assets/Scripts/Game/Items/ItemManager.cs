using System.Collections.Generic;
using UnityEngine;

namespace Game.Items
{
    public class ItemManager : MonoBehaviour
    {
        [Tooltip("아이템 컨테이너")] 
        public List<Item> items;

        private static ItemManager _instance;

        public static ItemManager Instance {
            get {
                if (_instance == null) {
                    return null;
                }
                
                return _instance;
            }
        }

        private void Awake() {
            if (_instance == null) {
                _instance = this;
                
                DontDestroyOnLoad(this.gameObject);
            }
            else {
                Destroy(this.gameObject);
            }
        }
    }
}