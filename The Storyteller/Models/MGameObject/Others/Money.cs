using System;
using System.Collections.Generic;
using System.Text;

namespace The_Storyteller.Models.MGameObject.Others
{
    class Money : GameObject
    {

        public Money(int quantity = 0)
        {
            Name = "Money";
            Quantity = quantity;
        }
        
    }
}
