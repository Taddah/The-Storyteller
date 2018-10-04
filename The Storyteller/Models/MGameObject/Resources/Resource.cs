using System;
using The_Storyteller.Models.MCharacter;

namespace The_Storyteller.Models.MGameObject.Resources
{
    public abstract class Resource : GameObject
    {
        public CharacterSkills AssociatedSkill { get; set; }
        public int HarvestableQuantity { get; set; } = 1;

        public Resource Harvest()
        {
            Random rnd = new Random();
            int toHarvest = rnd.Next(1, HarvestableQuantity);

            Resource resToGive = (Resource) MemberwiseClone();

            if (toHarvest > Quantity)
                toHarvest = Quantity;

            Quantity -= toHarvest;

            resToGive.Quantity = toHarvest;
            return resToGive;
        }
    }
}
