using System;
using UnityEngine;

namespace Game.Fields
{
    /// <summary>
    /// (다리 건설) 마우스 도착지점을 감지하는 클래스
    /// </summary>
    public class BridgeLinkPoint : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.transform.CompareTag("Bridge")) {
                var bridge = other.transform.GetComponent<BridgeObject>();

                if (bridge != null) {
                    bridge.parentTransform = this.transform;
                    bridge.entriesCount++;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.transform.CompareTag("Bridge")) {
                var bridge = other.transform.GetComponent<BridgeObject>();

                if (bridge != null) {
                    bridge.parentTransform = null;
                    bridge.entriesCount--;
                }
            }
        }
    }
}