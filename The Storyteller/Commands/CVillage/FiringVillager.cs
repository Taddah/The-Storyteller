using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Models.MCharacter;

namespace The_Storyteller.Commands.CVillage
{
    /// <summary>
    /// Seulement pour le roi ATM, permet de virer un villageois
    /// Il faudra par la suite penser à lui retirer ses attributions dans le village
    /// </summary>
    class FiringVillager
    {
        private readonly Dependencies dep;

        public FiringVillager(Dependencies d)
        {
            dep = d;
        }

        [Command("fireVillager")]
        public async Task FiringVillagerCommand(CommandContext ctx, params string[] name)
        {
            //Vérification de base character + guild + roi
            if (!dep.Entities.Characters.IsPresent(ctx.User.Id)
                || !dep.Entities.Guilds.IsPresent(ctx.Guild.Id)
                || dep.Entities.Characters.GetCharacterByDiscordId(ctx.User.Id).Profession != Profession.King)
            {
                return;
            }

            var strName = string.Join(" ", name);
            var character = dep.Entities.Characters.GetCharacterByDiscordId(ctx.User.Id);
            var village = dep.Entities.Villages.GetVillageByName(character.VillageName);
            var characterToFire = dep.Entities.Characters.GetCharacterByName(strName);

            if(characterToFire != null)
            {
                if (village.Villagers.Contains(characterToFire.Id))
                {
                    //On le vire
                    village.RemoveVillager(characterToFire);
                    var embedKicked = dep.Embed.CreateBasicEmbed(ctx.User, strName + " was kicked out of the village.");
                    await ctx.RespondAsync(embed: embedKicked);

                    var userToFire = await ctx.Client.GetUserAsync(characterToFire.Id);
                    var dmUser = await ctx.Client.CreateDmAsync(userToFire);
                    var embedFired = dep.Embed.CreateBasicEmbed(ctx.User, "You have been kicked out of the village of " + village.Name);
                    await dmUser.SendMessageAsync(embed: embedFired);
                    return;
                }
            }

            var embed = dep.Embed.CreateBasicEmbed(ctx.User, name + " does not exist or is not part of the village.");
            await ctx.RespondAsync(embed: embed);

        }
    }
}
