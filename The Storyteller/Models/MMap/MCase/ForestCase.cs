using System;
using The_Storyteller.Models.MGameObject.Resources;
using The_Storyteller.Models.MGameObject.Resources.Constructions;
using The_Storyteller.Models.MGameObject.Resources.Cookables;

namespace The_Storyteller.Models.MMap.MCase
{
    class ForestCase : Case
    {
        public override void GenerateResources()
        {
            Random rnd = new Random();

            //Si pas de ressource, on regenère de 0
            if (Resources.Count == 0)
            {
                Wood w = new Wood(rnd.Next(10, 100));
                base.Resources.Add(w);

                Stone s = new Stone(rnd.Next(0, 10));
                base.Resources.Add(s);

                Water wa = new Water(rnd.Next(0, 50));
                base.Resources.Add(wa);
            }
            else
            {
                foreach (Resource r in Resources)
                {
                    r.Quantity += r.Quantity / 2;
                }
            }
        }

        public override string GetTypeOfCase()
        {
            return "Forest";
        }

        public override bool IsBuildable()
        {
            return false;
        }
    }
}
