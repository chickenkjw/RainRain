using Game.Fields;
using Game.Player;
using UnityEngine;

namespace Game
{
    public class Water : MonoBehaviour
    {
        private float _height;
        
        private void Start() {
            transform.localScale =
                new Vector3(MapGenerator.Instance.floorWidth * 15, MapGenerator.Instance.floorHeight * 7, 1);

            transform.position = new Vector3(-450, -180, 0);

            _height = MapGenerator.Instance.floorHeight;
        }

        public void Rise() {
            transform.Translate(Vector3.up * _height);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            var obj = other.gameObject;

            if (obj.CompareTag("Player")) {
                obj.GetComponent<PlayerManager>().Drown();
            }
        }
    }
}