using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MGameObject;
using The_Storyteller.Models.MGameObject.Resources;
using The_Storyteller.Models.MMap;

namespace The_Storyteller.Commands.CCharacter
{
    class Harvest
    {
        private readonly Dependencies dep;

        public Harvest(Dependencies d)
        {
            dep = d;
        }

        [Command("harvest")]
        public async Task HarvestCommand(CommandContext ctx, string resourceName)
        {
            //Vérification de base character + guild && pas en DM
            if (!dep.Entities.Characters.IsPresent(ctx.User.Id)
                || (!ctx.Channel.IsPrivate) && !dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }

            //Récupérer nos infos (character, case)
            Character character = dep.Entities.Characters.GetCharacterByDiscordId(ctx.User.Id);
            Case currentCase = dep.Entities.Map.GetCase(character.Location);
            Resource res = currentCase.GetRessource(resourceName);

            //Ressource non trouvé
            if(res == null || res.Quantity == 0)
            {
                var embedNotFoud = dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("resourceNotAvailable"));
                await ctx.Channel.SendMessageAsync(embed: embedNotFoud);
                return;
            }

            //Sinon, ressource présente sur la map
            Inventory inventory = dep.Entities.Inventories.GetInventoryById(character.Id);

            Resource harvestedResource = res.Harvest();
            inventory.AddItem(harvestedResource);

            var embed = dep.Embed.CreateBasicEmbed(ctx.User, $"{harvestedResource.Name} récupéré : {harvestedResource.Quantity}");
            await ctx.Channel.SendMessageAsync(embed: embed);
            return;

        }
    }
}
