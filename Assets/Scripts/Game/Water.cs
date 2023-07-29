using System;
using Game.Fields;
using Game.Player;
using UnityEngine;

namespace Game
{
    public class Water : MonoBehaviour
    {
        private void Awake() {
            this.transform.localScale =
                new Vector3(MapGenerator.Instance.floorWidth * 15, MapGenerator.Instance.floorHeight * 7, 1);

            var width = this.GetComponent<SpriteRenderer>().bounds.size.x;
            var height = this.GetComponent<SpriteRenderer>().bounds.size.y;

            this.transform.position = new Vector3(-450, -180, 0);
        }

        public void Rise() {
            transform.Translate(Vector3.up * MapGenerator.Instance.floorHeight);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            var obj = other.gameObject;

            if (obj.CompareTag("Player")) {
                obj.GetComponent<PlayerManager>().Drown();
            }
        }
    }
}