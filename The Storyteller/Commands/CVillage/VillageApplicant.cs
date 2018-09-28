
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Entities.Tools;
using The_Storyteller.Models.MCharacter;

namespace The_Storyteller.Commands.CVillage
{
    /// <summary>
    /// Seulement pour le roi
    /// Affiche la liste des demandes en attentes avec possibilité de les accepter ou non
    /// </summary>
    class VillageApplicant
    {
        private readonly Dependencies dep;

        public VillageApplicant(Dependencies d)
        {
            dep = d;
        }

        [Command("villageApplicant")]
        public async Task VillageApplicantCommand(CommandContext ctx)
        {
            //Vérification de base character + guild + roi
            if (!dep.Entities.Characters.IsPresent(ctx.Member.Id)
                || !dep.Entities.Guilds.IsPresent(ctx.Guild.Id)
                || dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id).Profession != Profession.King)
            {
                return;
            }

            InteractivityModule interactivity = ctx.Client.GetInteractivityModule();
            var character = dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id);
            var village = dep.Entities.Villages.GetVillageByName(character.VillageName);

            List<string> waitingCharacter = new List<string>();
            foreach(ulong id in village.WaitingList)
            {
                var applicant = dep.Entities.Characters.GetCharacterByDiscordId(id);
                waitingCharacter.Add($"{applicant.Name} - Level {applicant.Level}");
            }

            List<CustomEmbedField> attributes = new List<CustomEmbedField>
            {
                //1 Infos général du personnage
                new CustomEmbedField()
                {
                    Name = "Inhabitant(s) waiting",
                    Attributes = waitingCharacter
                }
            };

            string title = $"List of applicants {character.Name}";
            string description = $"Type {Config.Instance.Prefix}accept [name] to accept, {Config.Instance.Prefix}refuse [name] to refuse";

            DiscordEmbedBuilder embed = dep.Embed.CreateDetailledEmbed(title, attributes, description, inline: true);

            DiscordDmChannel dm = await ctx.Member.CreateDmChannelAsync();
            await dm.SendMessageAsync(embed: embed);
            await ctx.RespondAsync($"{ctx.Member.Mention} private message sent !");

            while(village.WaitingList.Count > 0)
            {
                MessageContext msg = await interactivity.WaitForMessageAsync(xm => xm.Channel.Id == dm.Id, TimeSpan.FromMinutes(1));
                if (msg != null)
                {
                    var message = msg.Message.Content.Split(" ");
                    var command = message[0];
                    message = message.Skip(1).ToArray();

                    var name = string.Join(" ", message);

                    var applicant = dep.Entities.Characters.GetCharacterByName(name);

                    var applicantDiscordMember = await ctx.Client.GetUserAsync(applicant.Id);
                    var dmApplicant = await ctx.Client.CreateDmAsync(applicantDiscordMember);

                    if (applicant == null)
                    {
                        var embedError = dep.Embed.CreateBasicEmbed(ctx.Member, $"{name} was not found among the candidates.");
                        await dm.SendMessageAsync(embed: embedError);
                    }
                    else
                    {
                        if (command == Config.Instance.Prefix + "accept")
                        {
                            //Verifier si pas accepté ailleurs
                            if(applicant.VillageName != null)
                            {
                                var embedErrorVillage = dep.Embed.CreateBasicEmbed(ctx.Member, $"{name} is already member of another village");
                                await dm.SendMessageAsync(embed: embedErrorVillage);
                            }
                            else
                            {
                                village.WaitingList.Remove(applicant.Id);
                                village.AddInhabitant(applicant);
                                applicant.Profession = Profession.Villager;

                                var embedError = dep.Embed.CreateBasicEmbed(ctx.Member, $"{name} has been accepted");
                                await dm.SendMessageAsync(embed: embedError);

                                var embedAccepted = dep.Embed.CreateBasicEmbed(applicantDiscordMember, $"Congratulation, you have been accepted in village {village.Name}!");
                                await dmApplicant.SendMessageAsync(embed: embedAccepted);
                            }
                        }
                        else if (command == Config.Instance.Prefix + "refuse")
                        {
                            village.WaitingList.Remove(applicant.Id);

                            var embedError = dep.Embed.CreateBasicEmbed(ctx.Member, $"{name} has been refused");
                            await dm.SendMessageAsync(embed: embedError);

                            var embedAccepted = dep.Embed.CreateBasicEmbed(applicantDiscordMember, $"I am sorry to inform you that the village of {village.Name} has declined your candidature.");
                            await dmApplicant.SendMessageAsync(embed: embedAccepted);
                        }
                    }
                }
                else
                {
                    //Quitter la fonction
                    return;
                }

            }
        }
     }
}
