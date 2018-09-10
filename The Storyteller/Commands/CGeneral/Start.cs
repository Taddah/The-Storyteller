using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using The_Storyteller.Entities;
using The_Storyteller.Models.MCharacter;

namespace The_Storyteller.Commands.CGeneral
{
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
            if (_dep.Entities.Characters.IsPresent(ctx.Member.Id))
            {
                await ctx.RespondAsync(_dep.Resources.GetString("errorAlreadyRegistered"));
                return;
            }

            if (!_dep.Entities.Guilds.IsPresent(ctx.Guild.Id)) return;

            var interactivity = ctx.Client.GetInteractivityModule();

            //Create DM
            var channel = await ctx.Member.CreateDmChannelAsync();

            //Starting to create the character
            var c = new Character
            {
                DiscordID = ctx.Member.Id
            };

            //1 Get truename
            var embedTrueName = _dep.Embed.createEmbed(ctx.Member, _dep.Resources.GetString("startIntroAskTruename", c),
                _dep.Resources.GetString("startIntroInfoTruename", c));
            await channel.SendMessageAsync(embed: embedTrueName);
            var trueNameIsValid = false;
            do
            {
                var msgTrueName = await interactivity.WaitForMessageAsync(
                    xm => xm.Author.Id == ctx.User.Id && xm.ChannelId == channel.Id, TimeSpan.FromMinutes(1));
                if (msgTrueName != null)
                {
                    if (msgTrueName.Message.Content.Length <= 50
                        && !_dep.Entities.Characters.IsTrueNameTaken(msgTrueName.Message.Content)
                        && msgTrueName.Message.Content.Length > 2)
                    {
                        c.TrueName = msgTrueName.Message.Content;
                        _dep.Entities.Characters.AddCharacter(c);
                        trueNameIsValid = true;
                    }
                    else
                    {
                        var embedErrorTrueName = _dep.Embed.createEmbed(ctx.Member, _dep.Resources.GetString("startIntroTrueTaken"));
                        await channel.SendMessageAsync(embed: embedErrorTrueName);
                    }
                }
            } while (!trueNameIsValid);

            //2 Get Name
            var embedName = _dep.Embed.createEmbed(ctx.Member, _dep.Resources.GetString("startIntroAskName"),
                _dep.Resources.GetString("startIntroInfoName"));
            await channel.SendMessageAsync(embed: embedName);

            var msgName = await interactivity.WaitForMessageAsync(
                xm => xm.Author.Id == ctx.User.Id && xm.ChannelId == channel.Id, TimeSpan.FromMinutes(1));
            if (msgName != null) c.Name = msgName.Message.Content;

            //3 Get Sex
            var embedSex = _dep.Embed.createEmbed(ctx.Member, _dep.Resources.GetString("startIntroAskGender", c),
                _dep.Resources.GetString("startIntroInfoGender"));
            await channel.SendMessageAsync(embed: embedSex);

            var msgSex = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id &&
                                                                       (xm.Content.ToLower() == "male" ||
                                                                        xm.Content.ToLower() == "female" &&
                                                                        xm.ChannelId == channel.Id),
                TimeSpan.FromMinutes(1));
            if (msgSex != null)
            {
                if (msgSex.Message.Content.ToLower() == "male") c.Sex = Sex.Male;
                else c.Sex = Sex.Female;

                var embedFinal = _dep.Embed.createEmbed(ctx.Member, _dep.Resources.GetString("startIntroConclude", c));
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
                    UpgradePoint = 0
                };

                c.Id = _dep.Entities.Characters.GetCount();

                _dep.Entities.Characters.EditCharacter(c);
            }
        }
    }
}