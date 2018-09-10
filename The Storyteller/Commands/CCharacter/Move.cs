
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Entities.Tools;
using The_Storyteller.Models.MMap;

namespace The_Storyteller.Commands.CCharacter
{
    internal class Move
    {
        private readonly Dependencies dep;

        public Move(Dependencies d)
        {
            dep = d;
        }

        [Command("move")]
        public async Task MoveCommand(CommandContext ctx, string direction)
        {
            if (!dep.Entities.Characters.IsPresent(ctx.Member.Id)) return;

            var interactivity = ctx.Client.GetInteractivityModule();

            if (!IsDirectionOK(direction))
            {
                do
                {
                    var embedErrorDirection = dep.Embed.createEmbed(ctx.Member, dep.Resources.GetString("errorDirection"));
                    await ctx.RespondAsync(embed: embedErrorDirection);
                    var msgDirection = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id 
                    && xm.ChannelId == ctx.Channel.Id, TimeSpan.FromMinutes(1));

                    if (msgDirection != null)
                    {
                        //If response is a new command, stop this one
                        if (msgDirection.Message.Content.StartsWith(Config.Instance.Prefix)) return;
                        direction = msgDirection.Message.Content;
                    }
                } while (!IsDirectionOK(direction));
            }

            var character = dep.Entities.Characters.GetCharacterById(ctx.Member.Id);
            var newLocation = GetNewLocation(direction, character.Location);
            if (dep.Entities.Map.GetRegionByLocation(newLocation) == null)
            {
                //Generate new region
                var r = new Region
                {
                    Type = dep.Entities.Map.GetRandomRegionType()
                };

                var embedChooseName = dep.Embed.createEmbed(ctx.Member, dep.Resources.GetString("introductionChooseName", region: r));
                await ctx.RespondAsync(embed: embedChooseName);
                var regionName = "";
                var nameValid = false;

                do
                {
                    var msgGuildName = await interactivity.WaitForMessageAsync(
                        xm => xm.Author.Id == ctx.User.Id && xm.ChannelId == ctx.Channel.Id, TimeSpan.FromMinutes(1));
                    if (msgGuildName != null)
                    {
                        //If response is a new command, stop this one
                        if (msgGuildName.Message.Content.StartsWith(Config.Instance.Prefix)) return;
                        regionName = msgGuildName.Message.Content;
                    }

                    if (!dep.Entities.Map.IsRegionNameTaken(regionName) && regionName.Length > 3 && regionName.Length <= 50)
                        nameValid = true;
                    else
                    {
                        var embed = dep.Embed.createEmbed(ctx.Member, dep.Resources.GetString("regionNameTaken"));
                        await ctx.RespondAsync(embed: embed);
                    }
                } while (!nameValid);
                r = dep.Entities.Map.GenerateNewRegion(9, ctx.Guild.Id, regionName, r.Type);
            }

            //NOT WORKING

            if(dep.Entities.Map.GetRegionByLocation(newLocation).GetCase(newLocation).Type == CaseType.Water)
            {
                //Can' move here
                var embed = dep.Embed.createEmbed(ctx.Member, dep.Resources.GetString("errorDirectionWater"));
                await ctx.RespondAsync(embed: embed);
                return;
            }
            else
            {
                var lastCase = dep.Entities.Map.GetCase(character.Location);
                var currentRegion = dep.Entities.Map.GetRegionByLocation(newLocation);
                var newCase = currentRegion.GetCase(newLocation);

                lastCase.RemoveCharacter(character);
                newCase.AddNewCharacter(character);

                character.Location = newLocation;

                var embedCaseInfo = dep.Embed.createEmbed(ctx.Member, dep.Resources.GetString("caseInfo", region: currentRegion, mCase: newCase), 
                    dep.Resources.GetString("caseInfoDetails"));

                await ctx.RespondAsync(embed: embedCaseInfo);
            }

            
        }

        private bool IsDirectionOK(string direction)
        {
            if (direction.ToLower() == "north" || direction.ToLower() == "n") return true;
            if (direction.ToLower() == "south" || direction.ToLower() == "s") return true;
            if (direction.ToLower() == "east" || direction.ToLower() == "e") return true;
            if (direction.ToLower() == "west" || direction.ToLower() == "w") return true;
            return false;
        }

        private Location GetNewLocation(string direction, Location currentLocation)
        {
            var newLoc = new Location(currentLocation);
            switch (direction)
            {
                case "north": newLoc.YPosition += 1; break;
                case "n": newLoc.YPosition += 1; break;
                case "south": newLoc.YPosition -= 1; break;
                case "s": newLoc.YPosition -= 1; break;
                case "east": newLoc.XPosition += 1; break;
                case "e": newLoc.XPosition += 1; break;
                case "west": newLoc.XPosition -= 1; break;
                case "w": newLoc.XPosition -= 1; break;
            }
            return newLoc;
        }
    }
}