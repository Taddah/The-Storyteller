using System.Xml;
using The_Storyteller.Models.MGameObject.Resources;

namespace The_Storyteller.Models.MMap.MCase
{
    class DesertCase : Case
    {
        public override void GenerateResources()
        {
            Water w = new Water(999);
            base.Resources.Add(w);
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
