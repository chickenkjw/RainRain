using System.Collections.Generic;
using UnityEngine;

namespace Game.Fields
{
    public class Floor
    {
        public GameObject Object { get; set; }
        public Vector3 BuildPoint { get; set; }
        
        public bool ConnectBrokenBridge { get; set; }
        
        // 층의 좌표. x가 가로, y가 세로. 모두 0부터 시작.
        public Location Location { get; set; }
        
        // 상하좌우 다리가 연결되었는지 확인.
        public Dictionary<Direction, bool> Nodes = new() {
            { Direction.Left, false},
            { Direction.Right, false},
            { Direction.Up, false},
            { Direction.Down, false},
        };
    }
}