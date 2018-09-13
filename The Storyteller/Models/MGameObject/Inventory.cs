using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace The_Storyteller.Models.MGameObject
{
    class Inventory
    {
        [JsonProperty("inventory")]
        private List<GameObject> _gameObjects;

        public Inventory()
        {
            _gameObjects = new List<GameObject>();
        }

        public int GetMoney()
        {
            var money =  _gameObjects.SingleOrDefault(item => item.Name == "money");
            if (money == null)
            {
                AddMoney(0);
                return 0;
            }
            return money.Quantity;
            
        }

        public void AddMoney(int quantityToAdd)
        {
            if (!_gameObjects.Exists(item => item.Name == "money"))
                _gameObjects.Add(new GameObject()
                {
                    Name = "money",
                    Quantity = 0,
                    Value = 1
                });

            _gameObjects.SingleOrDefault(item => item.Name == "money").Quantity += quantityToAdd;
        }

        public void RemoveMoney(int quantityToRemove)
        {
            if (!_gameObjects.Exists(item => item.Name == "money"))
                _gameObjects.Add(new GameObject()
                {
                    Name = "money",
                    Quantity = 0,
                    Value = 1
                });

            if (_gameObjects.SingleOrDefault(item => item.Name == "money").Quantity >= quantityToRemove)
                _gameObjects.SingleOrDefault(item => item.Name == "money").Quantity -= quantityToRemove;
        }
    }
}
