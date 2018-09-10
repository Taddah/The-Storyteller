using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Models;
using The_Storyteller.Models.MMap;

namespace The_Storyteller.Commands.CGeneral
{
    internal class RegisterGuild
    {
        private readonly Dependencies dep;

        public RegisterGuild(Dependencies d)
        {
            dep = d;
        }

        [Command("registerGuild")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RegisterGuildCommand(CommandContext ctx)
        {
            if (dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                await ctx.RespondAsync(dep.Resources.GetString("errorGuildAlreadyRegistered"));
                return;
            }

            InteractivityModule interactivity = ctx.Client.GetInteractivityModule();

            string regionName = "";
            bool nameValid = false;

            //First look if a region is available
            Region r = dep.Entities.Map.GetAvailableRegion();
            if (r == null)
            {
                //Not ok, generate new region
                r = new Region
                {
                    Type = RegionType.Plain
                };

                DSharpPlus.Entities.DiscordEmbedBuilder embedChooseName = dep.Embed.createEmbed(ctx.Member, dep.Resources.GetString("introductionChooseName", region: r));
                await ctx.RespondAsync(embed: embedChooseName);

                do
                {

                    MessageContext msgGuildName = await interactivity.WaitForMessageAsync(
                        xm => xm.Author.Id == ctx.User.Id && xm.ChannelId == ctx.Channel.Id, TimeSpan.FromMinutes(1));
                    if (msgGuildName != null)
                    {
                        regionName = msgGuildName.Message.Content;
                    }

                    if (!dep.Entities.Map.IsRegionNameTaken(regionName) && regionName.Length > 3 && regionName.Length <= 50)
                    {
                        nameValid = true;
                    }
                    else
                    {
                        DSharpPlus.Entities.DiscordEmbedBuilder embed = dep.Embed.createEmbed(ctx.Member, dep.Resources.GetString("regionNameTaken"));
                        await ctx.RespondAsync(embed: embed);
                    }
                } while (!nameValid);
                r = dep.Entities.Map.GenerateNewRegion(9, ctx.Guild.Id, regionName, r.Type, true);
                Console.WriteLine(r.Name);
            }

            Console.WriteLine(r.Name);
            r.Id = ctx.Guild.Id;
            Guild g = new Guild
            {
                Id = ctx.Guild.Id,
                MemberCount = ctx.Guild.MemberCount,
                Name = ctx.Guild.Name,
                Region = r,
                SpawnLocation = new Location(0, 0)
            };
            dep.Entities.Guilds.AddGuild(g);

            DSharpPlus.Entities.DiscordEmbedBuilder embedEnd = dep.Embed.createEmbed(ctx.Member, dep.Resources.GetString("introductionGenFinish", region: r),
                dep.Resources.GetString("introTypeStart"), true);

            await ctx.RespondAsync(embed: embedEnd);
        }
    }
}