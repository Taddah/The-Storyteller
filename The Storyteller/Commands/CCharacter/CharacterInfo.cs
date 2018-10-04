using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Entities.Tools;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MGameObject;

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
            //Vérification de base character + guild && pas en DM
            if (!dep.Entities.Characters.IsPresent(ctx.User.Id)
                || (!ctx.Channel.IsPrivate) && !dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }

            string strName = "";
            Character c;
            DiscordMemberConverter dmc = new DiscordMemberConverter();

            DiscordDmChannel channel;
            if (!ctx.Channel.IsPrivate)
                channel = await ctx.Member.CreateDmChannelAsync();
            else
                channel = (DiscordDmChannel) ctx.Channel;

            //Pas de paramètre, on retourne les informations de l'auteur
            if (name.Length == 0)
            {
                c = dep.Entities.Characters.GetCharacterByDiscordId(ctx.User.Id);
                await channel.SendMessageAsync(embed: GetPersonalInfo(c));
                if (!ctx.Channel.IsPrivate)
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
                    if (!ctx.Channel.IsPrivate)
                        await ctx.RespondAsync($"{ctx.Member.Mention} private message sent !");
                    return;
                }
                else
                {
                    //Tester mention user
                    if (dmc.TryConvert(name[0], ctx, out DiscordMember member))
                    {
                        c = dep.Entities.Characters.GetCharacterByDiscordId(member.Id);
                    }
                    //Tester par nom
                    else
                    {
                        c = dep.Entities.Characters.GetCharacterByName(name[0]);
                    }
                }

                //Trouvé, retourne info basique
                if (c != null)
                {
                    DiscordEmbedBuilder embed = dep.Embed.CreateBasicEmbed(ctx.User
                        , dep.Dialog.GetString("publicInfo", c),
                    dep.Dialog.GetString("needTrueName"));
                    if (!ctx.Channel.IsPrivate)
                        await ctx.RespondAsync(embed: embed);
                    else
                        await channel.SendMessageAsync(embed: embed);
                    return;
                }
                else
                {
                    if (!ctx.Channel.IsPrivate)
                        await ctx.RespondAsync(dep.Dialog.GetString("errorCharacterUnknown"));
                    else
                        await channel.SendMessageAsync(dep.Dialog.GetString("errorCharacterUnknown"));
                    return;
                }
            }

            //Reconstruction du nom
            strName = string.Join(" ", name);


            c = dep.Entities.Characters.GetCharacterByTrueName(strName);
            //truename
            if (c != null)
            {
                await channel.SendMessageAsync(embed: GetDetailledInfo(c));
                if (!ctx.Channel.IsPrivate)
                    await ctx.RespondAsync($"{ctx.Member.Mention} private message sent !");
                return;
            }
            //Le nom
            else
            {
                c = dep.Entities.Characters.GetCharacterByName(strName);
                if(c != null)
                {
                    DiscordEmbedBuilder embed = dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("publicInfo", c),
                    dep.Dialog.GetString("needTrueName"));
                    if (!ctx.Channel.IsPrivate)
                        await ctx.RespondAsync(embed: embed);
                    else
                        await channel.SendMessageAsync(embed: embed);
                    return;
                }
            }

            if (!ctx.Channel.IsPrivate)
                await ctx.RespondAsync(dep.Dialog.GetString("errorCharacterUnknown"));
            else
                await channel.SendMessageAsync(dep.Dialog.GetString("errorCharacterUnknown"));
            return;
        }

        public DiscordEmbedBuilder GetPersonalInfo(Character c)
        {
            Inventory inv = dep.Entities.Inventories.GetInventoryById(c.Id);

            //Les skills du character
            //Affichage seulement si level > 0

            List<string> charSkills = new List<string>();
            foreach(CharacterSkills cs in c.Skills)
            {
                if (cs.Level > 0)
                {
                    charSkills.Add($"{cs.Name} - {cs.Level}  ({cs.Experience}/{cs.GetExperienceForNextLevel()})");
                }
            }

            List<CustomEmbedField> attributes = new List<CustomEmbedField>
            {

                //1 Infos général du personnage
                new CustomEmbedField()
                {
                    Name = "General informations",
                    Attributes = new List<string>
                    {
                        "Name: " + c.Name,
                        "TrueName: " + c.TrueName,
                        "Sex: " + c.Sex,
                        "Level: " + c.Level,
                        "Experience: " + c.Experience + "/XXXXX",
                        "Energy: " + c.Energy + "/" + c.MaxEnergy,
                        "Location: " + c.Location,
                        "Origin region: " + c.OriginRegionName,
                        "Profession: " + c.Profession
                    }
                },
                //2 Stats
                new CustomEmbedField()
                {
                    Name = "Statistics",
                    Attributes = new List<string>
                    {
                       "Health: " + c.Stats.Health +" / " + c.Stats.MaxHealth,
                       "Strength: " + c.Stats.Strength,
                       "Intelligence: " + c.Stats.Intelligence,
                       "Dexterity: " + c.Stats.Dexterity,
                       "Endurance: " + c.Stats.Endurance,
                       "Agility: " + c.Stats.Agility,
                       "Upgrade point available: " + c.Stats.UpgradePoint
                    }
                },
                //3 Inventory ?
                new CustomEmbedField()
                {
                    Name = "Inventory",
                    Attributes = new List<string>
                    {
                        "Money: " + inv.GetMoney(),
                        $"To view your inventory, type {Config.Instance.Prefix}inventory"
                    }
                },
                //4 Compétences
                new CustomEmbedField()
                {
                    Name = "Skills",
                    Attributes = charSkills
                }
            };

            string title = "Informations";

            DiscordEmbedBuilder embed = dep.Embed.CreateDetailledEmbed(title, attributes, inline: true);

            return embed;
        }

        public DiscordEmbedBuilder GetDetailledInfo(Character c)
        {
            List<CustomEmbedField> attributes = new List<CustomEmbedField>
            {
                //1 Infos général du personnage
                new CustomEmbedField()
                {
                    Name = "General informations",
                    Attributes = new List<string>
                    {
                        "Name: " + c.Name,
                        "TrueName: " + c.TrueName,
                        "Sex: " + c.Sex,
                        "Level: " + c.Level,
                        "Location: " + dep.Entities.Map.GetRegionByLocation(c.Location).Name,
                        "Origin region: " + c.OriginRegionName,
                        "Profession: " + c.Profession
                    }
                },
                //2 Stats
                new CustomEmbedField()
                {
                    Name = "Statistics",
                    Attributes = new List<string>
                    {
                       "Health: " + c.Stats.MaxHealth,
                       "Strength: " + c.Stats.Strength,
                       "Intelligence: " + c.Stats.Intelligence,
                       "Dexterity: " + c.Stats.Dexterity,
                       "Endurance: " + c.Stats.Endurance,
                       "Agility: " + c.Stats.Agility
                    }
                }
            };

            string title = "Informations";

            DiscordEmbedBuilder embed = dep.Embed.CreateDetailledEmbed(title, attributes, inline: true);

            return embed;
        }
    }
}