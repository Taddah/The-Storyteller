﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using The_Storyteller.Entities;

namespace The_Storyteller.Commands.CMap
{
    /// <summary>
    /// TODO
    /// Commande qui retourne des informations sur une case en particuler
    /// (Type de case, région associé, si contient village ..)
    /// </summary>
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

            await ctx.RespondAsync("hey");
        }
    }
}