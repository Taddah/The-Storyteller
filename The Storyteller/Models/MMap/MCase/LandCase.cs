using The_Storyteller.Models.MGameObject.Resources;

namespace The_Storyteller.Models.MMap.MCase
{
    class LandCase : Case
    {
        public override void GenerateResources()
        {
            Water w = new Water(999);
            base.Resources.Add(w);
        }

        public override bool IsBuildable()
        {
            return IsCentralCase();
        }

        public override string GetTypeOfCase()
        {
            return "Land";
        }
    }
}
