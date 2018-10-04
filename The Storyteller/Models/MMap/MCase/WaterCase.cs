using The_Storyteller.Models.MGameObject.Resources.Cookables;

namespace The_Storyteller.Models.MMap.MCase
{
    class WaterCase : Case
    {
        public override void GenerateResources()
        {
            Water w = new Water(999);
            base.Resources.Add(w);
        }

        public override bool IsBuildable()
        {
            return false;
        }

        public override string GetTypeOfCase()
        {
            return "Water";
        }
    }
}
