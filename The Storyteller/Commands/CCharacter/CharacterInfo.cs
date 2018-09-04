using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using The_Storyteller.Entities;
using The_Storyteller.Models;
using The_Storyteller.Models.MCharacter;

namespace The_Storyteller.Commands.CCharacter
{
    internal class CharacterInfo
    {
        private readonly Dependencies dep;

        public CharacterInfo(Dependencies d)
        {
            dep = d;
        }

        [Command("info")]
        public async Task CharacterInfoCommand(CommandContext ctx, [Description("The nickname to give to that user.")]
            params string[] name)
        {
            var strName = "";
            Character c;

            var dmc = new DiscordMemberConverter();
            var channel = await ctx.Member.CreateDmChannelAsync();


            if (name.Length == 0)
            {
                c = dep.Entities.Characters.GetCharacterById(ctx.Member.Id);
                await channel.SendMessageAsync(embed: GetPersonalInfo(c));
                await ctx.RespondAsync($"{ctx.Member.Mention} private message sent !");
                return;
            }

            if (name.Length == 1)
            {
                c = dep.Entities.Characters.GetCharacterByTrueName(name[0]);
                if (c != null)
                {
                    await channel.SendMessageAsync(embed: GetDetailledInfo(c));
                    await ctx.RespondAsync($"{ctx.Member.Mention} private message sent !");
                    return;
                }
                else if (dmc.TryConvert(name[0], ctx, out var member))
                {
                    c = dep.Entities.Characters.GetCharacterById(member.Id);
                    if (c == null)
                    {
                        await ctx.RespondAsync(dep.Resources.GetString("errorCharacterUnknown"));
                        return;
                    }

                    var embed = dep.Embed.createEmbed(dep.Resources.GetString("publicInfo", c),
                        dep.Resources.GetString("needTrueName"));
                    await channel.SendMessageAsync(embed: embed);
                    await ctx.RespondAsync($"{ctx.Member.Mention} private message sent !");
                    return;
                }

               
            }
            else
            {
                foreach (var s in name) strName += s + " ";
            }

            c = dep.Entities.Characters.GetCharacterByTrueName(strName);
            if (c != null)
            {
                await channel.SendMessageAsync(embed: GetDetailledInfo(c));
                await ctx.RespondAsync($"{ctx.Member.Mention} private message sent !");
                return;
            }

            await ctx.RespondAsync(dep.Resources.GetString("errorCharacterUnknown"));
        }

        public DiscordEmbedBuilder GetPersonalInfo(Character c)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"{c.Name} **{c.TrueName}**",
                Description = $"Energy : {c.Energy}/{c.MaxEnergy}",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Location : [{c.Location.XPosition}:{c.Location.YPosition}]"
                },
                ThumbnailUrl = "https://s33.postimg.cc/h0wkmhnm7/153046781869422717_1.png"
            };

            embed.AddField("Level", c.Level.ToString(), true);
            embed.AddField("Experience", $"{c.Experience} / XXXXX", true);
            embed.AddField("Endurance", c.Stats.Endurance.ToString(), true);
            embed.AddField("Strength", c.Stats.Strength.ToString(), true);
            embed.AddField("Intelligence", c.Stats.Intelligence.ToString(), true);
            embed.AddField("Agility", c.Stats.Agility.ToString(), true);
            embed.AddField("Dexterity", c.Stats.Dexterity.ToString(), true);
            embed.AddField("Upgrade points", c.Stats.UpgradePoint.ToString(), true);

            return embed;
        }

        public DiscordEmbedBuilder GetDetailledInfo(Character c)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"{c.Name} **{c.TrueName}**",
                Description = $"Level: {c.Level}",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "Location : [Near **VILLAGE X**"
                },
                ThumbnailUrl = "https://s33.postimg.cc/h0wkmhnm7/153046781869422717_1.png"
            };

            embed.AddField("Endurance", c.Stats.Endurance.ToString(), true);
            embed.AddField("Strength", c.Stats.Strength.ToString(), true);
            embed.AddField("Intelligence", c.Stats.Intelligence.ToString(), true);
            embed.AddField("Agility", c.Stats.Agility.ToString(), true);
            embed.AddField("Dexterity", c.Stats.Dexterity.ToString(), true);

            return embed;
        }
    }
}