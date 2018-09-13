using System.Collections.Generic;
using System.Linq;
using The_Storyteller.Models.MGameObject;

namespace The_Storyteller.Models.MCharacter
{
    class CharacterInventory
    {
        private readonly List<GameObject> _gameObjects;

        public CharacterInventory()
        {
            _gameObjects = new List<GameObject>();
        }

        public int GetMoney()
        {
            return _gameObjects.SingleOrDefault(item => item.Name == "money").Quantity;
        }

        public void AddMoney(int quantityToAdd)
        {
            if (_gameObjects.Exists(item => item.Name == "money"))
                _gameObjects.Add(new GameObject()
                {
                    Name = "money",
                    Quantity = 0
                });

            _gameObjects.SingleOrDefault(item => item.Name == "money").Quantity += quantityToAdd;
        }

        public void RemoveMoney(int quantityToRemove)
        {
            if (_gameObjects.Exists(item => item.Name == "money"))
                _gameObjects.Add(new GameObject()
                {
                    Name = "money",
                    Quantity = 0
                });

            if(_gameObjects.SingleOrDefault(item => item.Name == "money").Quantity >= quantityToRemove)
                _gameObjects.SingleOrDefault(item => item.Name == "money").Quantity -= quantityToRemove;
        }


    }
}