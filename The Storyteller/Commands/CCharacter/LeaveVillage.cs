using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MVillage;

namespace The_Storyteller.Commands.CCharacter
{
    /// <summary>
    /// Tout simplement quitter le village
    /// Le roi ne peut pas quitter son village
    /// </summary>
    class LeaveVillage
    {
        private readonly Dependencies dep;

        public LeaveVillage(Dependencies d)
        {
            dep = d;
        }

        [Command("leaveVillage")]
        public async Task JoinVillageCommand(CommandContext ctx)
        {
            //Vérification de base character + guild
            if (!dep.Entities.Characters.IsPresent(ctx.User.Id)
                || !dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }

            Character character = dep.Entities.Characters.GetCharacterByDiscordId(ctx.User.Id);

            if(character.VillageName != null && character.Profession != Profession.King)
            {
                Village village = dep.Entities.Villages.GetVillageByName(character.VillageName);

                character.VillageName = null;
                village.Villagers.Remove(character.Id);

                var embed = dep.Embed.CreateBasicEmbed(ctx.User, "You have left the village of " + village.Name);
                await ctx.RespondAsync(embed: embed);
            }
            else
            {
                var embed = dep.Embed.CreateBasicEmbed(ctx.User, "You cannot leave your village, either because you are the king or because you are not part of any village");
                await ctx.RespondAsync(embed: embed);
            }
        }
    }
}
