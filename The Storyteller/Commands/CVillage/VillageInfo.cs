using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using The_Storyteller.Entities;

namespace The_Storyteller.Commands.CVillage
{
    [Group("villageinfo", CanInvokeWithoutSubcommand = true)]
    class VillageInfo
    {
        private readonly Dependencies _dep;

        public VillageInfo(Dependencies d)
        {
            _dep = d;
        }

        public async Task ExecuteGroupAsync(CommandContext ctx)
        {
            await ctx.RespondAsync("without subcommand");
        }

        [Command("building")]
        public async Task VillageInfoCommand(CommandContext ctx)
        {
            await ctx.RespondAsync("building info");
        }
    }
}
