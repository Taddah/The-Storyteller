namespace The_Storyteller.Models.MMap
{
    public enum CaseType
    {
        Water,
        Forest,
        Desert,
        Mountain,
        Land
    }

    internal class Case
    {
        public Location Location { get; set; }
        public CaseType Type { get; set; }
        public bool IsAvailable { get; set; }

        public bool IsBuildable()
        {
            if (Type == CaseType.Mountain
                || Type == CaseType.Water
                || Type == CaseType.Desert) return false;

            return true;
        }
    }

   
}