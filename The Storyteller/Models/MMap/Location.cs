using System;

namespace The_Storyteller.Models.MMap
{
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

        public int XPosition { get; set; }
        public int YPosition { get; set; }

        public override string ToString()
        {
            return $"[{XPosition};{YPosition}]";
        }

        public override bool Equals(object obj)
        {
            var loc = (Location)obj;
            if (loc == null) return false;
            if (loc.XPosition != XPosition) return false;
            if (loc.YPosition != YPosition) return false;

            return true;
        }
    }
}