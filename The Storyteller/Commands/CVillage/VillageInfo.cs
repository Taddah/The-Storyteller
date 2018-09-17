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

        public async Task ExecuteGroupAsync(CommandContext ctx)
        {
            //Vérification de base character + guild
            if (!dep.Entities.Characters.IsPresent(ctx.Member.Id)
                || !dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }

            var character = dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id);
            if(character.VillageName == null)
            {
                var embed = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Dialog.GetString("errorNotPartOfVillage"));
                await ctx.RespondAsync(embed: embed);
                return;
            }


            var village = dep.Entities.Villages.GetVillageByName(character.VillageName);
            var embedVillageDetailled = GetVillageInfo(village, true);

            var dm = await ctx.Member.CreateDmChannelAsync();
            await dm.SendMessageAsync(embed: embedVillageDetailled);
        }

        [Command("building")]
        public async Task VillageInfoBuildingCommand(CommandContext ctx)
        {
            await ctx.RespondAsync("building info");
        }

        public DiscordEmbedBuilder GetVillageInfo(Village v, bool detailled)
        {
            var king = dep.Entities.Characters.GetCharacterByDiscordId(v.KingId);

            List<string> inventoryList = new List<string>();
            inventoryList.Add("Treasury: " + v.Inventory.GetMoney());
            inventoryList.Add("----------------");
            foreach (GameObject go in v.Inventory.GetItems())
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
            foreach (Building building in v.getBuildings())
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
                    Name = "Inventory",
                    Attributes = inventoryList
                },
                new CustomEmbedField()
                {
                    Name = "Villagers",
                    Attributes = charactersList
                },
                new CustomEmbedField()
                {
                    Name = "Buildings",
                    Attributes = buildingList
                }
            };

            string title = "Village Informations";

            DiscordEmbedBuilder embed = dep.Embed.CreateDetailledEmbed(title, attributes, inline: true);

            return embed;
        }
    }
}
