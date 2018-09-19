using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Entities.Tools;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MMap;

namespace The_Storyteller.Commands.CMap
{
    /// <summary>
    /// TODO
    /// Commande qui retourne des informations sur une case en particuler
    /// (Type de case, région associé, si contient village ..)
    /// </summary>
    internal class CaseInfo
    {
        private readonly Dependencies dep;

        public CaseInfo(Dependencies d)
        {
            dep = d;
        }

        [Command("caseinfo")]
        public async Task CaseInfoCommand(CommandContext ctx)
        {
            //Vérification de base character + guild
            if (!dep.Entities.Characters.IsPresent(ctx.Member.Id)
                || !dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }
            Character character = dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id);
            Region currentRegion = dep.Entities.Map.GetRegionByLocation(character.Location);
            Case currentCase =currentRegion.GetCase(character.Location);

            //1 genral information
            List<string> genInfo = new List<string>()
                    {
                        "Case type: " + currentCase.Type,
                        "Location: " + currentCase.Location,
                        "Region: " + currentRegion.Name
                    };

            if(currentCase.VillageId > 0)
            {
                var village = dep.Entities.Villages.GetVillageById(currentCase.VillageId);
                genInfo.Add("Village: " + village.Name);
            }
            else if (currentCase.IsBuildable())
            {
                genInfo.Add("You can build a village here.");
            }

            //2 Character présent
            List<string> charInfo = new List<string>();
            foreach(ulong charId in currentCase.GetCharactersOnCase())
            {
                var c = dep.Entities.Characters.GetCharacterByDiscordId(charId);
                charInfo.Add(c.Name + " - " + c.Sex);
            }

            //3 Case alentours
            List<string> caseInfo = new List<string>();

            var northCase = dep.Entities.Map.GetCase(new Location(currentCase.Location.XPosition, currentCase.Location.YPosition + 1));
            if (northCase != null)
            {
                caseInfo.Add($"North {northCase.Location} - {northCase.Type}");
            }
            else
            {
                caseInfo.Add("North : unknown");
            }

            var southCase = dep.Entities.Map.GetCase(new Location(currentCase.Location.XPosition, currentCase.Location.YPosition - 1));
            if (southCase != null)
            {
                caseInfo.Add($"South {southCase.Location} - {southCase.Type}");
            }
            else
            {
                caseInfo.Add("South : unknown");
            }

            var eastCase = dep.Entities.Map.GetCase(new Location(currentCase.Location.XPosition + 1, currentCase.Location.YPosition));
            if (eastCase != null)
            {
                caseInfo.Add($"East {eastCase.Location} - {eastCase.Type}");
            }
            else
            {
                caseInfo.Add("East : unknown");
            }

            var westCase = dep.Entities.Map.GetCase(new Location(currentCase.Location.XPosition - 1, currentCase.Location.YPosition));
            if (westCase != null)
            {
                caseInfo.Add($"West {westCase.Location} - {westCase.Type}");
            }
            else
            {
                caseInfo.Add("West : unknown");
            }


            List<CustomEmbedField> attributes = new List<CustomEmbedField>
            {
                new CustomEmbedField()
                {
                    Name = "Current location",
                    Attributes = genInfo
                },
                new CustomEmbedField()
                {
                    Name = "Character present",
                    Attributes = charInfo
                },
                new CustomEmbedField()
                {
                    Name = "Case around",
                    Attributes = caseInfo
                }
            };

            var title = "Case information";
            var embed = dep.Embed.CreateDetailledEmbed(title, attributes, inline: true);

            var dm = await ctx.Member.CreateDmChannelAsync();
            await dm.SendMessageAsync(embed: embed);
            await ctx.RespondAsync($"{ctx.Member.Mention} private message sent !");

        }
    }
}