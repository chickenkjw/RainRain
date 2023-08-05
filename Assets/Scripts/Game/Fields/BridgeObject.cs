using UnityEngine;

namespace Game.Fields
{
    public class BridgeObject: MonoBehaviour
    {
        private Location _connectedLoc;
        
        public Location ConnectedLoc {
            get => _connectedLoc;
            set {
                _connectedLoc = value;
                inverseLoc.X = value.X - 1;
                inverseLoc.Y = value.Y;
            }
        }

        public Location inverseLoc;
    }
}