using System;
using The_Storyteller.Models.MGameObject.Resources;
using The_Storyteller.Models.MGameObject.Resources.Constructions;

namespace The_Storyteller.Models.MMap.MCase
{
    class DesertCase : Case
    {
        public override void GenerateResources()
        {
            Random rnd = new Random();

            //Si pas de ressource, on regenère de 0
            if(Resources.Count == 0)
            {
                Sand s = new Sand(rnd.Next(50, 100));
                base.Resources.Add(s);
            }

            //Desert, que du sable !

            
        }

        public override string GetTypeOfCase()
        {
            return "Desert";
        }

        public override bool IsBuildable()
        {
            return false;
        }
    }
}
