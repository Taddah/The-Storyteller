using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using The_Storyteller.Models.MGameObject;

namespace The_Storyteller.Entities.Game
{
    class InventoryManager
    {
        private readonly string _filename;

        private List<Inventory> _inventories;

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
            return new List<Inventory>();
        }

        public async Task SaveToFile()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("inventories");

            foreach(Inventory inv in _inventories)
            {
                XmlElement inventory1 = doc.CreateElement("inventory");
                inventory1.SetAttribute("id", inv.Id.ToString());

                foreach(GameObject go in inv.GetAllGameObjects())
                {
                    XmlElement xmlGo = go.Seralize(doc);
                    inventory1.AppendChild(xmlGo);
                }

                root.AppendChild(inventory1);
            }

            doc.AppendChild(root);

            await Task.Factory.StartNew(delegate
            {
                using (var sw = new StreamWriter(_filename))
                {
                    doc.Save(sw);
                }
            });

            
        }

        public Inventory GetInventoryById(ulong id)
        {
            return _inventories.SingleOrDefault(inv => inv.Id == id);
        }

        public void AddInventory(Inventory inv)
        {
            if (Exist(inv)) return;

            _inventories.Add(inv);
        }

        public bool Exist(Inventory inv)
        {
            return _inventories.Exists(inventory => inventory.Id == inv.Id);
        }
    }
}
