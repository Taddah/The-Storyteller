using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using The_Storyteller.Entities;
using The_Storyteller.Models.MMap;

namespace The_Storyteller.Commands.CMap
{
    /// <summary>
    /// Commande qui retourne des informations sur sur une région
    /// </summary>
    internal class RegionInfo
    {
        private readonly Dependencies dep;

        public RegionInfo(Dependencies d)
        {
            dep = d;
        }

        [Command("regioninfo")]
        public async Task RegionInfoCommand(CommandContext ctx, params string[] name)
        {
            if (!dep.Entities.Characters.IsPresent(ctx.Member.Id)) return;

            Region r;

            if (name.Length > 0)
            {
                var strName = "";
                foreach (string s in name) strName += s + " ";
                strName = strName.Remove(strName.Length - 1);
                r = dep.Entities.Map.GetRegionByName(strName);
            }
            else
            {
                var userLoc = dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id).Location;
                r = dep.Entities.Map.GetRegionByLocation(userLoc);
            }

            if (r == null)
            {
                await ctx.RespondAsync(embed: dep.Embed.CreateBasicEmbed(ctx.Member, dep.Dialog.GetString("regionNotFound"),
                    withPicture: true));
                return;
            }

           var embed = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Dialog.GetString("regionDescription", region: r));

           await ctx.RespondAsync(embed: embed);
            
        }


    }
}