using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using The_Storyteller.Models.MGameObject.Equipment.Weapons;
using The_Storyteller.Models.MGameObject.GOResource;
using The_Storyteller.Models.MGameObject.Others;

namespace The_Storyteller.Models.MGameObject
{
    public class Inventory
    {

        public ulong Id { get; set; }

        private List<GameObject> _gameObjects;


        public Inventory()
        {
            _gameObjects = new List<GameObject>();
        }

        public int GetMoney()
        {
            if (!(_gameObjects.SingleOrDefault(item => item is Money) is Money money))
            {
                AddMoney(0);
                return 0;
            }
            return money.Quantity;
        }

        public void AddMoney(int quantityToAdd)
        {
            if (!_gameObjects.Exists(item => item is Money))
            {
                _gameObjects.Add(new Money());
            }

            _gameObjects.SingleOrDefault(item => item is Money).Quantity += quantityToAdd;
        }

        public void RemoveMoney(int quantityToRemove)
        {
            if (!_gameObjects.Exists(item => item is Money))
            {
                _gameObjects.Add(new Money());
            }

            if (_gameObjects.SingleOrDefault(item => item is Money).Quantity >= quantityToRemove)
            {
                _gameObjects.SingleOrDefault(item => item is Money).Quantity -= quantityToRemove;
            }
        }

        /// <summary>
        /// Return everything but money
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetItems()
        {
            return _gameObjects.Where(item => !(item is Money)).ToList();
        }

        public List<GameObject> GetAllGameObjects()
        {
            return _gameObjects;
        }

        /// <summary>
        /// Return item of type resource
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetResources()
        {
            return _gameObjects.Where(item => item is Resource).ToList();

        }

        /// <summary>
        /// Return item of type equipment
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetEquipment()
        {
            return _gameObjects.Where(item => item is Weapon).ToList();

        }

        public bool HasObjectAndQuantity(GameObject gameObject, int quantity)
        {
           return _gameObjects.Exists(go => go.GetType() == gameObject.GetType() && go.Quantity >= quantity);
    
        }

        public GameObject GetItemByName(string name)
        {
            return _gameObjects.SingleOrDefault(go => go.Name.ToLower() == name.ToLower());
        }

        public GameObject GetItemByType(GameObject gameObject)
        {
            return _gameObjects.SingleOrDefault(go => go.GetType() == gameObject.GetType());
        }

        public GameObject GetGOAndRemoveFromInventory(string name, int quantity)
        {
            GameObject go = GetItemByName(name);
            if (go == null)
            {
                return null;
            }

            return GetGOAndRemoveFromInventory(go, quantity);
        }


        public GameObject GetGOAndRemoveFromInventory(GameObject gameObject, int quantity)
        {
            if (!HasObjectAndQuantity(gameObject, quantity))
            {
                return null;
            }

            GameObject gameobject = _gameObjects.SingleOrDefault(go => go.GetType() == gameObject.GetType());
            gameobject.Quantity -= quantity;

            //Si quantité = 0 et pas money, on vire de l'inventaire
            if (!(gameObject is Money) && gameobject.Quantity == 0)
            {
                _gameObjects.Remove(gameobject);
            }

            //Sinon, copie 
            GameObject newGameObject = gameObject.Clone() as GameObject;
            newGameObject.Quantity = quantity;

            return newGameObject;
        }

        public void AddItems(List<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                GameObject go = GetItemByType(gameObject);
                if (go == null)
                {
                    _gameObjects.Add(gameObject);
                }
                else
                {
                    go.Quantity += gameObject.Quantity;
                }
            }
        }

        public void AddItem(GameObject gameObject)
        {
            GameObject go = GetItemByType(gameObject);
            if (go == null)
            {
                _gameObjects.Add(gameObject);
            }
            else
            {
                go.Quantity += gameObject.Quantity;
            }
        }

        public void RemoveGameObject(GameObject go)
        {
            _gameObjects.Remove(go);
        }

        public void RemoveGameObject(List<GameObject> gos)
        {
            foreach(GameObject go in gos)
                _gameObjects.Remove(go);
        }
    }
}
