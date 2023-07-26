using Game.Fields;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public GameObject environment;

        public static GameManager Instance {
            get {
                if (null == _instance) {
                    return null;
                }
                
                return _instance;
            }
        }
        
        void Awake() {
            if (_instance == null) {
                _instance = this;
                
                DontDestroyOnLoad(this.gameObject);
            }
            else {
                Destroy(this.gameObject);
            }
        }

        void Start() {
            MapGenerator.Instance.GenerateMap();
        }

        public void DestroyMap() {
            Transform[] children = environment.transform.GetChild(0).GetComponentsInChildren<Transform>();

            if (children.Length > 0) {
                for (int i = 1; i < children.Length; i++) {
                    Destroy(children[i].gameObject);
                }
            }
        }

        public void ReGenerateMap() {
            MapGenerator.Instance.GenerateMap();
        }
    }
}
