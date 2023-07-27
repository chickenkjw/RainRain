﻿using System;
using Game.Fields;
using Game.Player;
using UnityEngine;

namespace Game
{
    public class Water : MonoBehaviour
    {
        public void Rise() {
            transform.Translate(Vector3.up * MapGenerator.Instance.floorHeight);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            var obj = other.gameObject;

            if (obj.CompareTag("Player")) {
                obj.GetComponent<PlayerManager>().Drawn();
            }
        }
    }
}