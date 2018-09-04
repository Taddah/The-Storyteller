
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Storyteller.Entities;
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
                    var embedErrorDirection = dep.Embed.createEmbed(dep.Resources.GetString("errorDirection"));
                    await ctx.RespondAsync(embed: embedErrorDirection);
                    var msgDirection = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id 
                    && xm.ChannelId == ctx.Channel.Id, TimeSpan.FromMinutes(1));

                    if (msgDirection != null && !msgDirection.Message.Content.StartsWith('!')) direction = msgDirection.Message.Content;
                } while (!IsDirectionOK(direction));
            }

            var character = dep.Entities.Characters.GetCharacterById(ctx.Member.Id);
            var newLocation = GetNewLocation(direction, character.Location);

            if(dep.Entities.Map.GetRegionByLocation(newLocation) == null)
            {
                await ctx.RespondAsync("new region in " + newLocation.ToString());
            }
            else
            {
                await ctx.RespondAsync("moving " + character.Name + " to " + newLocation.ToString());
            }

            character.Location = newLocation;
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
            switch (direction)
            {
                case "north": currentLocation.YPosition += 1; break;
                case "n": currentLocation.YPosition += 1; break;
                case "south": currentLocation.YPosition -= 1; break;
                case "s": currentLocation.YPosition -= 1; break;
                case "east": currentLocation.XPosition += 1; break;
                case "e": currentLocation.XPosition += 1; break;
                case "west": currentLocation.XPosition -= 1; break;
                case "w": currentLocation.XPosition -= 1; break;
            }
            return currentLocation;
        }
    }
}