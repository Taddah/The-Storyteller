using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using The_Storyteller.Entities;

namespace The_Storyteller.Commands.CVillage
{
    class CreateVillage
    {
        private readonly Dependencies _dep;

        public CreateVillage(Dependencies d)
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
        }
    }
}
