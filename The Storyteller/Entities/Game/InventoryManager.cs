using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using The_Storyteller.Models.MGameObject;
using The_Storyteller.Models.MGameObject.Equipment.Weapons;
using The_Storyteller.Models.MGameObject.Resources;
using The_Storyteller.Models.MGameObject.Others;
using The_Storyteller.Models.MGameObject.Resources.Ore;

namespace The_Storyteller.Entities.Game
{
    /// <summary>
    /// Gestionnaire d'inventaire, la sauvegarde est en xml pour le moment et écrire à la main
    /// Afin de gérer la liste d'abstract
    /// Classe équipé d'une factory à modifier à chauque ajout d'un nouveau Gameobject afin de pouvoir reconstruire
    /// l'object correment depuis le fichier de sauvegarde
    /// </summary>
    class InventoryManager
    {
        private readonly string _filename;

        private readonly List<Inventory> _inventories;

        public InventoryManager(string filename)
        {
            _filename = filename;
            _inventories = LoadFromFile();

            Task task = new Task(async () => await DoPeriodicCharacterSave());
            task.Start();
        }

        private async Task DoPeriodicCharacterSave()
        {
            while (true)
            {
                try
                {
                    await SaveToFile();
                }
                catch (IOException)
                {

                }

                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }

        public List<Inventory> LoadFromFile()
        {
            if (!File.Exists(_filename))
            {
                return new List<Inventory>();
            }

            List<Inventory> listInv = new List<Inventory>();

            XmlDocument doc = new XmlDocument();
            doc.Load(_filename);

            //get inventories
            XmlNode inventories = doc.GetElementsByTagName("inventories").Item(0);

            //Pour chaque inventaire
            foreach (XmlElement inventory in inventories.ChildNodes)
            {
                if (!ulong.TryParse(inventory.GetAttribute("id"), out ulong id))
                {
                    break;
                }

                Inventory inv = new Inventory
                {
                    Id = id
                };

                foreach (XmlElement obj in inventory.ChildNodes)
                {
                    GameObject gameObject = BuildGameObject(obj);
                    inv.AddItem(gameObject);
                }
                listInv.Add(inv);
            }

            return listInv;
        }

        public async Task SaveToFile()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("inventories");

            foreach (Inventory inv in _inventories)
            {
                XmlElement inventory1 = doc.CreateElement("inventory");
                inventory1.SetAttribute("id", inv.Id.ToString());

                foreach (GameObject go in inv.GetAllGameObjects())
                {
                    XmlElement xmlGo = go.Serialize(doc);
                    inventory1.AppendChild(xmlGo);
                }

                root.AppendChild(inventory1);
            }

            doc.AppendChild(root);

            await Task.Factory.StartNew(delegate
            {
                using (StreamWriter sw = new StreamWriter(_filename))
                {
                    doc.Save(sw);
                }
            });
        }

        public GameObject BuildGameObject(XmlElement element)
        {
            string type = element.GetAttribute("type");
            switch (type)
            {
                case "money": return Money.Build(element);
                case "wood": return Wood.Build(element);
                case "weapon": return Weapon.Build(element);
                case "coal": return Coal.Build(element);
                case "copper": return Copper.Build(element);
                case "gold": return Gold.Build(element);
                case "iron": return Iron.Build(element);
                case "silver": return Silver.Build(element);
                case "leather": return Leather.Build(element);
                case "meat": return Meat.Build(element);
                case "sand": return Sand.Build(element);
                case "stone": return Stone.Build(element);
                case "water": return Water.Build(element);
                case "wheat": return Wheat.Build(element);
                default: return null;
            }
        }

        public Inventory GetInventoryById(ulong id)
        {
            return _inventories.SingleOrDefault(inv => inv.Id == id);
        }

        public void AddInventory(Inventory inv)
        {
            if (Exist(inv))
            {
                return;
            }

            _inventories.Add(inv);
        }

        public bool Exist(Inventory inv)
        {
            return _inventories.Exists(inventory => inventory.Id == inv.Id);
        }
    }
}
