using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Entities.Tools;
using The_Storyteller.Models.MGameObject;
using The_Storyteller.Models.MVillage;

namespace The_Storyteller.Commands.CVillage
{
    [Group("villageinfo", CanInvokeWithoutSubcommand = true)]
    class VillageInfo
    {
        private readonly Dependencies dep;

        public VillageInfo(Dependencies d)
        {
            dep = d;
        }

        public async Task ExecuteGroupAsync(CommandContext ctx, params string[] name)
        {
            //Vérification de base character + guild
            if (!dep.Entities.Characters.IsPresent(ctx.Member.Id)
                || !dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }

            var character = dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id);
            var villageName = character.VillageName;
            var detailled = true;
               
            
            if (name.Length > 0)
            {
                villageName = "";
                foreach (string s in name)
                    villageName += s + " ";
                villageName = villageName.Remove(villageName.Length - 1);
                detailled = false;

            }

            Village village = dep.Entities.Villages.GetVillageByName(villageName);

            if (village == null)
            {
                var embed = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Dialog.GetString("errorNotPartOfVillage"));
                await ctx.RespondAsync(embed: embed);
                return;
            }

            var  embedVillage = GetVillageInfo(village, detailled);

            if (detailled)
            {
                var dm = await ctx.Member.CreateDmChannelAsync();
                await dm.SendMessageAsync(embed: embedVillage);
                await ctx.RespondAsync($"{ctx.Member.Mention} private message sent !");
            }
            else
            {
                await ctx.RespondAsync(embed: embedVillage);
            }
            
        }

        [Command("building")]
        public async Task VillageInfoBuildingCommand(CommandContext ctx)
        {
            await ctx.RespondAsync("building info");
        }

        public DiscordEmbedBuilder GetVillageInfo(Village v, bool detailled)
        {
            var king = dep.Entities.Characters.GetCharacterByDiscordId(v.KingId);
            var inventory = dep.Entities.Inventories.GetInventoryById(v.Id);

            List<string> inventoryList = new List<string>();
            inventoryList.Add("Treasury: " + inventory.GetMoney());
            inventoryList.Add("----------------");
            foreach (GameObject go in inventory.GetItems())
            {
                inventoryList.Add($"{go.Name} -  Quantity: {go.Quantity}");
            }

            List<string> charactersList = new List<string>();
            foreach (int cId in v.GetInhabitants())
            {
                var c = dep.Entities.Characters.GetCharacterById(cId);
                charactersList.Add($"{c.Name} - Profession: {c.Profession}");
            }

            List<string> buildingList = new List<string>();
            foreach (Building building in v.GetBuildings())
            {
                var c = dep.Entities.Characters.GetCharacterById(building.ProprietaryId);
                buildingList.Add($"{building.Name} - Level: {building.Level} - Proprietary: {c.Name}");
            }

            List<CustomEmbedField> attributes = new List<CustomEmbedField>
            {
                //1 Infos général du personnage
                new CustomEmbedField()
                {
                    Name = "General informations",
                    Attributes = new List<string>
                    {
                        "Name: " + v.Name,
                        "Region: " + v.RegionName,
                        "King: " + king.Name,
                        "Access: " + v.VillagePermission
                    }
                },
                new CustomEmbedField()
                {
                    Name = "Villagers",
                    Attributes = charactersList
                }
            };

            if (detailled)
            {
                attributes.Add(new CustomEmbedField()
                {
                    Name = "Buildings",
                    Attributes = buildingList
                });

                attributes.Add(new CustomEmbedField()
                {
                    Name = "Inventory",
                    Attributes = inventoryList
                });
            }

            string title = "Village Informations";

            DiscordEmbedBuilder embed = dep.Embed.CreateDetailledEmbed(title, attributes, inline: true);

            return embed;
        }
    }
}
