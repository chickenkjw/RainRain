using System;
using UnityEngine;

namespace Game.Fields
{
    /// <summary>
    /// 다리 오브젝트에 붙일 스크립트
    /// </summary>
    public class BridgeObject : MonoBehaviour
    {
        [HideInInspector] public Transform parentTransform;
        [HideInInspector] public int entriesCount;
        [HideInInspector] public Location Location;

        private void Start() {
            entriesCount = 0;
        }
    }
}