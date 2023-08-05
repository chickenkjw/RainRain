using System;

namespace Game.Fields
{
    [Serializable]
    public class Location
    {
        public int X { get; set; }
        public int Y { get; set; }

        public bool Equals(Location location) {
            return location.X == X && location.Y == Y;
        }
    }
}