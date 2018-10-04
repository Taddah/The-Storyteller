using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MCharacter.MCharacterSkills;
using The_Storyteller.Models.MGameObject.Equipment.Weapons;
using The_Storyteller.Models.MGameObject.Resources.Constructions;

namespace The_Storyteller.Commands.CCharacter
{
    /// <summary>
    /// Commande pour créer un nouveau Character
    /// Celui ci sera placé sur la région associé au discord où
    /// cette commande est exécutée
    /// </summary>
    internal class Start
    {
        private readonly Dependencies dep;

        public Start(Dependencies d)
        {
            dep = d;
        }

        [Command("start")]
        public async Task StartCommand(CommandContext ctx)
        {
            //Vérification de base
            if (dep.Entities.Characters.IsPresent(ctx.User.Id))
            {
                await ctx.RespondAsync(dep.Dialog.GetString("errorAlreadyRegistered"));
                return;
            }
            if (!dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }

            InteractivityModule interactivity = ctx.Client.GetInteractivityModule();

            //Créer Direct Channel (MP)
            DiscordDmChannel channel = await ctx.Member.CreateDmChannelAsync();

            //Création du Character
            Character c = new Character
            {
                Id = ctx.User.Id
            };

            //1 On récupère le truename puis on enregistre directement pour éviter les doublons
            DiscordEmbedBuilder embedTrueName = dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("startIntroAskTruename"),
                dep.Dialog.GetString("startIntroInfoTruename"));
            await channel.SendMessageAsync(embed: embedTrueName);
            bool trueNameIsValid = false;
            do
            {
                MessageContext msgTrueName = await interactivity.WaitForMessageAsync(
                    xm => xm.Author.Id == ctx.User.Id && xm.ChannelId == channel.Id, TimeSpan.FromMinutes(1));
                if (msgTrueName != null)
                {
                    if (msgTrueName.Message.Content.Length <= 50
                        && !dep.Entities.Characters.IsTrueNameTaken(msgTrueName.Message.Content)
                        && msgTrueName.Message.Content.Length > 2)
                    {
                        c.TrueName = dep.Dialog.RemoveMarkdown(msgTrueName.Message.Content);

                        dep.Entities.Characters.AddCharacter(c);
                        trueNameIsValid = true;
                    }
                    else
                    {
                        DiscordEmbedBuilder embedErrorTrueName = dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("startIntroTrueTaken"));
                        await channel.SendMessageAsync(embed: embedErrorTrueName);
                    }
                }
            } while (!trueNameIsValid);

            //2 On demande le nom
            DiscordEmbedBuilder embedName = dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("startIntroAskName"),
                dep.Dialog.GetString("startIntroInfoName"));
            await channel.SendMessageAsync(embed: embedName);

            MessageContext msgName = await interactivity.WaitForMessageAsync(
                xm => xm.Author.Id == ctx.User.Id && xm.ChannelId == channel.Id, TimeSpan.FromMinutes(1));
            if (msgName != null)
            {
                c.Name = dep.Dialog.RemoveMarkdown(msgName.Message.Content);
            }

            //3 Puis finalement le sexe
            DiscordEmbedBuilder embedSex = dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("startIntroAskGender"),
                dep.Dialog.GetString("startIntroInfoGender"));
            await channel.SendMessageAsync(embed: embedSex);

            MessageContext msgSex = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id &&
                                                                       (xm.Content.ToLower() == "male" ||
                                                                        xm.Content.ToLower() == "female" &&
                                                                        xm.ChannelId == channel.Id),
                                                                        TimeSpan.FromMinutes(1));
            if (msgSex != null)
            {
                if (msgSex.Message.Content.ToLower() == "male")
                {
                    c.Sex = Sex.Male;
                }
                else
                {
                    c.Sex = Sex.Female;
                }
            }
            else
            {
                c.Sex = Sex.Male;
            }

            //Si le nom a bien été rentré, on créer le personnage
            if (c.Name != null)
            {
                DiscordEmbedBuilder embedFinal = dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("startIntroConclude", c));
                await channel.SendMessageAsync(embed: embedFinal);

                c.Level = 1;
                c.Energy = 100;
                c.MaxEnergy = 100;
                c.Location = dep.Entities.Guilds.GetGuildById(ctx.Guild.Id).SpawnLocation;
                c.Stats = new CharacterStats
                {
                    Endurance = 1,
                    Strength = 1,
                    Intelligence = 1,
                    Agility = 1,
                    Dexterity = 1,
                    Health = 100,
                    MaxHealth = 100,
                    UpgradePoint = 0
                };


                //INVENTAIRE
                CharacterInventory inv = new CharacterInventory
                {
                    Id = c.Id
                };
                inv.AddMoney(500);
                inv.AddItem(new Wood(10));
                inv.AddItem(new Weapon()
                {
                    Name = "Awesome sword",
                    Quantity = 1,
                    AttackDamage = 10,
                    CraftsmanId = 100,
                    Hand = 2
                });

                c.Skills.Add(new LoggerSkill());
                

                dep.Entities.Inventories.AddInventory(inv);

                c.OriginRegionName = dep.Entities.Map.GetRegionByLocation(dep.Entities.Guilds.GetGuildById(ctx.Guild.Id).SpawnLocation).Name;
                c.Profession = Profession.Peasant;

                dep.Entities.Map.GetCase(c.Location).AddNewCharacter(c);
                dep.Entities.Characters.EditCharacter(c);
            }
            //Sinon on supprime celui qui avait commencé à être créer
            else
            {
                dep.Entities.Characters.DeleteCharacter(c.Id);
            }
        }
    }
}