using System;

namespace The_Storyteller.Models.MMap
{
    /// <summary>
    /// Jeu de coordonnées tout simple X Y
    /// </summary>
    internal class Location
    {
        public Location()
        {
        }

        public Location(int xPos, int yPos)
        {
            XPosition = xPos;
            YPosition = yPos;
        }

        public Location(Location loc)
        {
            XPosition = loc.XPosition;
            YPosition = loc.YPosition;
        }

        public int XPosition { get; set; }
        public int YPosition { get; set; }

        public override string ToString()
        {
            return $"[{XPosition};{YPosition}]";
        }

        public bool Equals(Location loc)
        {
            if (loc == null) return false;
            if (loc.XPosition != XPosition) return false;
            if (loc.YPosition != YPosition) return false;

            return true;
        }
    }
}