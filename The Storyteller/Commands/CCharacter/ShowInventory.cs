using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Entities.Tools;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MGameObject;

namespace The_Storyteller.Commands.CCharacter
{
    class ShowInventory
    {
        private readonly Dependencies dep;

        public ShowInventory(Dependencies d)
        {
            dep = d;
        }

        [Command("inventory")]
        public async Task ShowInventoryCommand(CommandContext ctx)
        {
            //Vérification de base character + guild
            if (!dep.Entities.Characters.IsPresent(ctx.Member.Id))
            {
                return;
            }

            //1 Récupérer le Character et son inventaire
            Character character = dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id);
            Inventory inventory = dep.Entities.Inventories.GetInventoryById(character.Id);

            List<string> resourceList = new List<string>();
            foreach (GameObject go in inventory.GetResources())
            {
                resourceList.Add($"{go.Name} -  Quantity: {go.Quantity}");
            }

            List<string> equipmentList = new List<string>();
            foreach (GameObject go in inventory.GetEquipment())
            {
                equipmentList.Add($"{go.Name} -  Quantity: {go.Quantity}");
            }

            List<string> itemsList = new List<string>();
            foreach (GameObject go in inventory.GetItems())
            {
                //itemsList.Add($"'{go.Name}' -  Quantity: {go.Quantity}");
            }


            List<CustomEmbedField> attributes = new List<CustomEmbedField>
            {

                //1 Infos général du personnage
                new CustomEmbedField()
                {
                    Name = "Resources",
                    Attributes = resourceList
                },
                //2 equipments
                new CustomEmbedField()
                {
                    Name = "Equipments",
                    Attributes = equipmentList
                },
                //2 items
                new CustomEmbedField()
                {
                    Name = "Items",
                    Attributes = itemsList
                },
                //4 Currently equipped
                new CustomEmbedField()
                {
                    Name = "Currently equipped",
                    Attributes = new List<string>
                    {
                        "Head: ",
                        "Right hand: ",
                        "Left hand: ",
                        "Body: ",
                        "Feet: "
                    }
                }
            };

            string title = $"Inventory of {character.Name}";
            string description = $"Money: {inventory.GetMoney()}";

            DiscordEmbedBuilder embed = dep.Embed.CreateDetailledEmbed(title, attributes, description, inline: true);

            DiscordDmChannel dm = await ctx.Member.CreateDmChannelAsync();
            await dm.SendMessageAsync(embed: embed);
            await ctx.RespondAsync($"{ctx.Member.Mention} private message sent !");


        }
    }
}
