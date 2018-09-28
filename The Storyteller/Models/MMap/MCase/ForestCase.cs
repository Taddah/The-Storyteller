using The_Storyteller.Models.MGameObject.Resources;

namespace The_Storyteller.Models.MMap.MCase
{
    class ForestCase : Case
    {
        public override void GenerateResources()
        {
            Water w = new Water(999);
            base.Resources.Add(w);
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
