using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Entities.Tools;
using The_Storyteller.Models.MCharacter;

namespace The_Storyteller.Commands.CCharacter
{
    /// <summary>
    /// Commande pour afficher les informations d'un Character
    /// Paramètre possible : Mention (@user), TrueName ou ID
    /// Si Mention ou ID, retourne détails basique
    /// Si truename, retourne plus de détail
    /// </summary>
    internal class CharacterInfo
    {
        private readonly Dependencies dep;

        public CharacterInfo(Dependencies d)
        {
            dep = d;
        }

        [Command("info")]
        public async Task CharacterInfoCommand(CommandContext ctx, params string[] name)
        {
            //Auteur pas inscrit,
            if (!dep.Entities.Characters.IsPresent(ctx.Member.Id))
            {
                return;
            }

            string strName = "";
            Character c;
            DiscordMemberConverter dmc = new DiscordMemberConverter();
            DiscordDmChannel channel = await ctx.Member.CreateDmChannelAsync();

            //Pas de paramètre, on retourne les informations de l'auteur
            if (name.Length == 0)
            {
                c = dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id);
                await channel.SendMessageAsync(embed: GetPersonalInfo(c));
                await ctx.RespondAsync($"{ctx.Member.Mention} private message sent !");
                return;
            }

            //Nom sans espace, tester si trueName en premier
            if (name.Length == 1)
            {
                //Recherche par truename
                c = dep.Entities.Characters.GetCharacterByTrueName(name[0]);
                if (c != null)
                {
                    await channel.SendMessageAsync(embed: GetDetailledInfo(c));
                    await ctx.RespondAsync($"{ctx.Member.Mention} private message sent !");
                    return;
                }
                else
                {
                    //Tester par ID
                    if (int.TryParse(name[0], out int id))
                    {
                        c = dep.Entities.Characters.GetCharacterById(id);
                    }
                    //Tester mention user
                    else if (dmc.TryConvert(name[0], ctx, out DiscordMember member))
                    {
                        c = dep.Entities.Characters.GetCharacterByDiscordId(member.Id);
                    }
                }

                //Trouvé, retourne info basique
                if (c != null)
                {
                    DiscordEmbedBuilder embed = dep.Embed.createEmbed(ctx.Member, dep.Resources.GetString("publicInfo", c),
                    dep.Resources.GetString("needTrueName"));
                    await channel.SendMessageAsync(embed: embed);
                    await ctx.RespondAsync($"{ctx.Member.Mention} private message sent !");
                    return;
                }
                else
                {
                    await ctx.RespondAsync(dep.Resources.GetString("errorCharacterUnknown"));
                    return;
                }
            }

            //Reconstruction du nom
            foreach (string s in name)
            {
                strName += s + " ";
            }

            strName = strName.Remove(strName.Length - 1);


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
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = $"{c.Name} **{c.TrueName}**",
                Description = $"Energy : {c.Energy}/{c.MaxEnergy}, ID : {c.Id}",
                Color = Config.Instance.Color,
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
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = $"{c.Name} **{c.TrueName}**",
                Description = $"Level: {c.Level}, ID : {c.Id}",
                Color = Config.Instance.Color,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Location : {dep.Entities.Map.GetRegionByLocation(c.Location).Name}"
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