﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Entities.Tools;
using The_Storyteller.Models;
using The_Storyteller.Models.MMap;

namespace The_Storyteller.Commands.CGuild
{
    /// <summary>
    /// Commande pour rajouter un serveur disord au bot, nécessaire pour générer la map
    /// qui sera associé à ce discord
    /// </summary>
    internal class RegisterGuild
    {
        private readonly Dependencies dep;

        public RegisterGuild(Dependencies d)
        {
            dep = d;
        }

        [Command("registerGuild")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RegisterGuildCommand(CommandContext ctx)
        {
            //Ce serveur discord est déjà enregistré et dispose de sa propre map
            if (dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                await ctx.RespondAsync(dep.Dialog.GetString("errorGuildAlreadyRegistered"));
                return;
            }

            InteractivityModule interactivity = ctx.Client.GetInteractivityModule();

            string regionName = "";
            bool nameValid = false;

            //Avant de générer une nouvelle map, on regarde si une déjà générée par 
            //l'exploration est disponible
            Region r = dep.Entities.Map.GetAvailableRegion();

            //Pas disponible, génération d'une nouvelle map de type Plain et forcément habitable
            if (r == null)
            {
                r = new Region
                {
                    Type = RegionType.Plain,
                    Id = ctx.Guild.Id
                };

                //Comme souvent, choix du nom et vérification
                DiscordEmbedBuilder embedChooseName = dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("introductionChooseName", region: r));
                await ctx.RespondAsync(embed: embedChooseName);

                do
                {
                    MessageContext msgGuildName = await interactivity.WaitForMessageAsync(
                        xm => xm.Author.Id == ctx.User.Id && xm.ChannelId == ctx.Channel.Id, TimeSpan.FromMinutes(1));
                    if (msgGuildName != null)
                    {
                        //Nouvelle commande, on annule
                        if (msgGuildName.Message.Content.StartsWith(Config.Instance.Prefix))
                        {
                            return;
                        }
                        else
                        {
                            regionName = msgGuildName.Message.Content;
                        }

                        //Enlever *, ` et _
                        regionName = dep.Dialog.RemoveMarkdown(regionName);
                    }

                    if (!dep.Entities.Map.IsRegionNameTaken(regionName) && regionName.Length > 3 && regionName.Length <= 50)
                    {
                        nameValid = true;
                    }
                    else
                    {
                        DiscordEmbedBuilder embed = dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("regionNameTaken"));
                        await ctx.RespondAsync(embed: embed);
                    }
                } while (!nameValid);
                r = dep.Entities.Map.GenerateNewRegion(9, ctx.Guild.Id, regionName, r.Type, forceValable: true);
            }
            
            //Enregistrement du serveur discord associé à sa région
            Guild g = new Guild
            {
                Id = ctx.Guild.Id,
                MemberCount = ctx.Guild.MemberCount,
                Name = ctx.Guild.Name,
                RegionName = r.Name,
                SpawnLocation = r.GetCentralCase().Location
            };
            dep.Entities.Guilds.AddGuild(g);

            //Message de bienvenue (YAY)
            DiscordEmbedBuilder embedEnd = dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("introductionGenFinish", region: r),
                dep.Dialog.GetString("introTypeStart"), true);

            await ctx.RespondAsync(embed: embedEnd);
        }
    }
}