using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Models.MCharacter;

namespace The_Storyteller.Commands.CCharacter
{
    /// <summary>
    /// Commande pour créer un nouveau Character
    /// Celui ci sera placé sur la région associé au discord où
    /// cette commande est exécutée
    /// </summary>
    internal class Start
    {
        private readonly Dependencies _dep;

        public Start(Dependencies d)
        {
            _dep = d;
        }

        [Command("start")]
        public async Task StartCommand(CommandContext ctx)
        {
            //Vérification de base
            if (_dep.Entities.Characters.IsPresent(ctx.Member.Id))
            {
                await ctx.RespondAsync(_dep.Resources.GetString("errorAlreadyRegistered"));
                return;
            }
            if (!_dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }

            InteractivityModule interactivity = ctx.Client.GetInteractivityModule();

            //Créer Direct Channel (MP)
            DiscordDmChannel channel = await ctx.Member.CreateDmChannelAsync();

            //Création du Character
            Character c = new Character
            {
                DiscordID = ctx.Member.Id
            };

            //1 On récupère le truename puis on enregistre directement pour éviter les doublons
            DiscordEmbedBuilder embedTrueName = _dep.Embed.CreateBasicEmbed(ctx.Member, _dep.Resources.GetString("startIntroAskTruename"),
                _dep.Resources.GetString("startIntroInfoTruename"));
            await channel.SendMessageAsync(embed: embedTrueName);
            bool trueNameIsValid = false;
            do
            {
                MessageContext msgTrueName = await interactivity.WaitForMessageAsync(
                    xm => xm.Author.Id == ctx.User.Id && xm.ChannelId == channel.Id, TimeSpan.FromMinutes(1));
                if (msgTrueName != null)
                {
                    if (msgTrueName.Message.Content.Length <= 50
                        && !_dep.Entities.Characters.IsTrueNameTaken(msgTrueName.Message.Content)
                        && msgTrueName.Message.Content.Length > 2)
                    {
                        c.TrueName = _dep.Resources.RemoveMarkdown(msgTrueName.Message.Content);

                        _dep.Entities.Characters.AddCharacter(c);
                        trueNameIsValid = true;
                    }
                    else
                    {
                        DiscordEmbedBuilder embedErrorTrueName = _dep.Embed.CreateBasicEmbed(ctx.Member, _dep.Resources.GetString("startIntroTrueTaken"));
                        await channel.SendMessageAsync(embed: embedErrorTrueName);
                    }
                }
            } while (!trueNameIsValid);

            //2 On demande le nom
            DiscordEmbedBuilder embedName = _dep.Embed.CreateBasicEmbed(ctx.Member, _dep.Resources.GetString("startIntroAskName"),
                _dep.Resources.GetString("startIntroInfoName"));
            await channel.SendMessageAsync(embed: embedName);

            MessageContext msgName = await interactivity.WaitForMessageAsync(
                xm => xm.Author.Id == ctx.User.Id && xm.ChannelId == channel.Id, TimeSpan.FromMinutes(1));
            if (msgName != null)
            {
                c.Name = _dep.Resources.RemoveMarkdown(msgName.Message.Content);
            }

            //3 Puis finalement le sexe
            DiscordEmbedBuilder embedSex = _dep.Embed.CreateBasicEmbed(ctx.Member, _dep.Resources.GetString("startIntroAskGender"),
                _dep.Resources.GetString("startIntroInfoGender"));
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
            else c.Sex = Sex.Male;

            //Si le nom a bien été rentré, on créer le personnage
            if(c.Name != null)
            { 
                DiscordEmbedBuilder embedFinal = _dep.Embed.CreateBasicEmbed(ctx.Member, _dep.Resources.GetString("startIntroConclude", c));
                await channel.SendMessageAsync(embed: embedFinal);

                c.Level = 1;
                c.Energy = 100;
                c.MaxEnergy = 100;
                c.Location = _dep.Entities.Guilds.GetGuildById(ctx.Guild.Id).SpawnLocation;
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

                c.Inventory = new CharacterInventory();
                c.Inventory.AddMoney(500);
                
                c.OriginRegionName = _dep.Entities.Map.GetRegionByLocation(_dep.Entities.Guilds.GetGuildById(ctx.Guild.Id).SpawnLocation).Name;

                c.Id = _dep.Entities.Characters.GetCount();
                _dep.Entities.Characters.EditCharacter(c);
            }
            //Sinon on supprime celui qui avait commencé à être créer
            else
            {
                _dep.Entities.Characters.DeleteCharacter(c.DiscordID);
            }
        }
    }
}