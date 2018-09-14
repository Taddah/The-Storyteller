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

        public List<GameObject> GetItems()
        {
            return _gameObjects.Where(item => item.Name != "money").ToList();
        }

        public bool HasObjectAndQuantity(string objectName, int quantity)
        {
            return _gameObjects.Exists(go => go.Name.ToLower() == objectName.ToLower() && go.Quantity >= quantity);
        }

        public GameObject GetGOAndRemoveFromInventory(string name, int quantity)
        {
            if (!HasObjectAndQuantity(name, quantity)) return null;

            var gameobject = _gameObjects.SingleOrDefault(go => go.Name.ToLower() == name.ToLower());
            gameobject.Quantity -= quantity;

            //Si quantité = 0 et pas money, on vire de l'inventaire
            if (name != "money" && gameobject.Quantity == 0) _gameObjects.Remove(gameobject);

            return new GameObject()
            {
                Name = gameobject.Name,
                Quantity = quantity
            };
        }

        public void AddItems(List<GameObject> gameObjects)
        {
            foreach(GameObject go in gameObjects)
            {
                //Si item existe déjà, on additionne les quantité
                if (_gameObjects.Exists(item => item.Name == go.Name))
                    _gameObjects.Single(item => item.Name == go.Name).Quantity += go.Quantity;
                else
                    _gameObjects.Add(go);
            }
        }
    }
}
