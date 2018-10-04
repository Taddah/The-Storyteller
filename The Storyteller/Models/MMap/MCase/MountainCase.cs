using System;
using The_Storyteller.Models.MGameObject.Resources;
using The_Storyteller.Models.MGameObject.Resources.Constructions;
using The_Storyteller.Models.MGameObject.Resources.Cookables;
using The_Storyteller.Models.MGameObject.Resources.Ore;

namespace The_Storyteller.Models.MMap.MCase
{
    class MountainCase : Case
    {
        public override void GenerateResources()
        {
            Random rnd = new Random();

            //Si pas de ressource, on regenère de 0
            if (Resources.Count == 0)
            {
                Wood w = new Wood(rnd.Next(0, 10));
                base.Resources.Add(w);

                Stone s = new Stone(rnd.Next(0, 30));
                base.Resources.Add(s);

                Water wa = new Water(rnd.Next(0, 10));
                base.Resources.Add(wa);

                Coal co = new Coal(rnd.Next(0, 30));
                base.Resources.Add(co);

                Copper cop = new Copper(rnd.Next(0, 30));
                base.Resources.Add(cop);

                Gold go = new Gold(rnd.Next(0, 5));
                base.Resources.Add(go);

                Iron ir = new Iron(rnd.Next(0, 30));
                base.Resources.Add(ir);

                Silver si = new Silver(rnd.Next(0, 10));
                base.Resources.Add(si);
            }
            else
            {
                foreach (Resource r in Resources)
                {
                    r.Quantity += r.Quantity / 2;
                }
            }
        }

        public override bool IsBuildable()
        {
            return false;
        }

        public override string GetTypeOfCase()
        {
            return "Mountain";
        }
    }
}
