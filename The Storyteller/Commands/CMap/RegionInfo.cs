using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using The_Storyteller.Entities;
using The_Storyteller.Models.MMap;

namespace The_Storyteller.Commands.CMap
{
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
            Region r;

            if (name.Length > 0)
            {
                r = dep.Entities.Map.GetRegionByName(name[0]);
            }
            else
            {
                var userLoc = dep.Entities.Characters.GetCharacterById(ctx.Member.Id).Location;
                r = dep.Entities.Map.GetRegionByLocation(userLoc);
            }

            if (r == null)
            {
                await ctx.RespondAsync(embed: dep.Embed.createEmbed(dep.Resources.GetString("regionNotFound"),
                    withPicture: true));
                return;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"**{r.Name}** type: {r.Type}",
                Description = $"Size: {r.GetSize()}"
            };

            var land = 0;
            var mountain = 0;
            var water = 0;
            var desert = 0;
            var forest = 0;

            foreach(Case c in r.GetAllCases())
            {
                if (c.Type == CaseType.Desert) desert++;
                if (c.Type == CaseType.Water) water++;
                if (c.Type == CaseType.Mountain) mountain++;
                if (c.Type == CaseType.Forest) forest++;
                if (c.Type == CaseType.Land) land++;
            }

            embed.AddField($"Land:", $"{land}");
            embed.AddField($"Water:", $"{water}");
            embed.AddField($"Forest:", $"{forest}");
            embed.AddField($"Desert:", $"{desert}");
            embed.AddField($"Moutain:", $"{mountain}");

            embed = dep.Embed.createEmbed(dep.Resources.GetString("regionDescription", region: r));

            await ctx.RespondAsync(embed: embed);
        }
    }
}