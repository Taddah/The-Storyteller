using System;
using System.Collections.Generic;
using System.Text;

namespace The_Storyteller.Models.MVillage.Buildings
{
    class House : Building
    {
        public int ComfortLevel {
            get { return Level * 10; }
            set { }
        }
    }
}
