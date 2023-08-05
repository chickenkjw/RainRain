using System.Collections.Generic;
using UnityEngine;

namespace Game.Items
{

    public class ItemManager : MonoBehaviour
    {
        

        [Tooltip("아이템 컨테이너")] 
        public List<GameObject> items;

        [Tooltip("다리 오브젝트")] 
        public List<GameObject> bridgeObject;

        
    }
}