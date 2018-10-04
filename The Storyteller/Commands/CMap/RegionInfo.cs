using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
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
            //Vérification de base character + guild && pas en DM
            if (!dep.Entities.Characters.IsPresent(ctx.User.Id)
                || (!ctx.Channel.IsPrivate) && !dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }

            Region r;

            if (name.Length > 0)
            {
                string strName = string.Join(" ", name);
                r = dep.Entities.Map.GetRegionByName(strName);
            }
            else
            {
                Location userLoc = dep.Entities.Characters.GetCharacterByDiscordId(ctx.User.Id).Location;
                r = dep.Entities.Map.GetRegionByLocation(userLoc);
            }

            if (r == null)
            {
                await ctx.RespondAsync(embed: dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("regionNotFound"),
                    withPicture: true));
                return;
            }

            DiscordEmbedBuilder embed = dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("regionDescription", region: r));

            await ctx.Channel.SendMessageAsync(embed: embed);

        }
    }
}