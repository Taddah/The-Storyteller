using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Entities.Game;
using The_Storyteller.Entities.Tools;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MMap;

namespace The_Storyteller.Commands.CCharacter
{
    /// <summary>
    /// Commande pour les déplacements
    /// Paramètre acceptés : string (north, n, south, s, east, e, west, w)
    /// Génère une nouvelle région lorsqu'un character se déplace à une Location n'étant rattaché à aucune région
    /// @Todo : retirer energie
    /// </summary>
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
            if (!dep.Entities.Characters.IsPresent(ctx.Member.Id))
            {
                return;
            }

            InteractivityModule interactivity = ctx.Client.GetInteractivityModule();

            //Si direction indiqué n'est pas reconnu, on redemande tant que résultat pas bon
            if (GetDirection(direction) == Direction.Unknown)
            {
                do
                {
                    DiscordEmbedBuilder embedErrorDirection = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Resources.GetString("errorDirection"));
                    await ctx.RespondAsync(embed: embedErrorDirection);
                    MessageContext msgDirection = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id
                    && xm.ChannelId == ctx.Channel.Id, TimeSpan.FromMinutes(1));

                    if (msgDirection != null)
                    {
                        //La réponse est une nouvelle commande, on oublie celle là
                        if (msgDirection.Message.Content.StartsWith(Config.Instance.Prefix))
                        {
                            return;
                        }
                        //Sinon on retest la validité
                        else
                        {
                            direction = msgDirection.Message.Content;
                        }
                    }
                } while (GetDirection(direction) == Direction.Unknown);
            }

            Character character = dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id);
            Region currentRegion = dep.Entities.Map.GetRegionByLocation(character.Location);
            Location newLocation = GetNewLocation(GetDirection(direction), character.Location);

            //Region non découverte => Nouvelle génération
            if (dep.Entities.Map.GetRegionByLocation(newLocation) == null)
            {
                //Type aléatoire, la région généré ne sera pas forcément valide pour un village
                Region r = new Region
                {
                    Type = dep.Entities.Map.GetRandomRegionType()
                };

                //Choix du nom de la région, demander tant qu'il n'est pas valide
                DiscordEmbedBuilder embedChooseName = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Resources.GetString("introductionChooseName", region: r));
                await ctx.RespondAsync(embed: embedChooseName);
                string regionName = "";
                bool nameValid = false;

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
                        regionName = dep.Resources.RemoveMarkdown(regionName);
                    }

                    if (!dep.Entities.Map.IsRegionNameTaken(regionName) && regionName.Length > 3 && regionName.Length <= 50)
                    {
                        nameValid = true;
                    }
                    else
                    {
                        DiscordEmbedBuilder embed = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Resources.GetString("regionNameTaken"));
                        await ctx.RespondAsync(embed: embed);
                    }
                } while (!nameValid);

                //Calculer la prochine case centrale pour générer la région au bon endroit
                Location nextMapLoc = dep.Entities.Map.GetCentralCaseByDirection(currentRegion.GetCentralCase(), GetDirection(direction));
                //Générer la région avec pour centre nextMapLoc
                r = dep.Entities.Map.GenerateNewRegion(9, ctx.Guild.Id, regionName, r.Type, nextMapLoc);
                DiscordEmbedBuilder embedRegionDiscovered = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Resources.GetString("regionDiscovered", region: r));
                await ctx.RespondAsync(embed: embedRegionDiscovered);
            }

            //Eau, impossible d'y aller (pour le moment)
            if (dep.Entities.Map.GetRegionByLocation(newLocation).GetCase(newLocation).Type == CaseType.Water)
            {
                DiscordEmbedBuilder embed = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Resources.GetString("errorDirectionWater"));
                await ctx.RespondAsync(embed: embed);
                return;
            }
            //Sinon, on bouge
            else
            {
                Region newRegion = dep.Entities.Map.GetRegionByLocation(newLocation);
                Case lastCase = dep.Entities.Map.GetCase(character.Location);
                Case newCase = newRegion.GetCase(newLocation);

                lastCase.RemoveCharacter(character);
                newCase.AddNewCharacter(character);

                character.Location = newLocation;

                DiscordEmbedBuilder embedCaseInfo = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Resources.GetString("caseInfo", region: newRegion, mCase: newCase),
                     dep.Resources.GetString("caseInfoDetails"));

                await ctx.RespondAsync(embed: embedCaseInfo);
            }


        }

        /// <summary>
        /// Convertit direction string en enum Direction (North, South, East, West)
        /// Retourne unknow si string non reconnu
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Direction GetDirection(string direction)
        {
            if (direction.ToLower() == "north" || direction.ToLower() == "n")
            {
                return Direction.North;
            }

            if (direction.ToLower() == "south" || direction.ToLower() == "s")
            {
                return Direction.South;
            }

            if (direction.ToLower() == "east" || direction.ToLower() == "e")
            {
                return Direction.East;
            }

            if (direction.ToLower() == "west" || direction.ToLower() == "w")
            {
                return Direction.West;
            }

            return Direction.Unknown;
        }

        /// <summary>
        /// Calculer la nouvelle Location d'un Character à partir de la direction demandé
        /// et de sa Location actuelle
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="currentLocation"></param>
        /// <returns></returns>
        private Location GetNewLocation(Direction direction, Location currentLocation)
        {
            Location newLoc = new Location(currentLocation);
            switch (direction)
            {
                case Direction.North: newLoc.YPosition += 1; break;
                case Direction.South: newLoc.YPosition -= 1; break;
                case Direction.East: newLoc.XPosition += 1; break;
                case Direction.West: newLoc.XPosition -= 1; break;
            }
            return newLoc;
        }
    }
}