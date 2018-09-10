using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Storyteller.Entities;

namespace The_Storyteller.Commands.CMap
{
    internal class CaseInfo
    {
        private readonly Dependencies dep;

        public CaseInfo(Dependencies d)
        {
            dep = d;
        }

        [Command("caseinfo")]
        public async Task CaseInfoCommand(CommandContext ctx)
        {
            if (!dep.Entities.Characters.IsPresent(ctx.Member.Id)) return;


        }
    }
}