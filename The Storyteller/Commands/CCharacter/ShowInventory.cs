using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Entities.Tools;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MGameObject;

namespace The_Storyteller.Commands.CCharacter
{
    class ShowInventory
    {
        private readonly Dependencies _dep;

        public ShowInventory(Dependencies d)
        {
            _dep = d;
        }

        [Command("inventory")]
        public async Task ShowInventoryCommand(CommandContext ctx)
        {
            //Vérification de base character + guild
            if (!_dep.Entities.Characters.IsPresent(ctx.Member.Id)
                || !_dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }

            //1 Récupérer le Character et son inventaire
            Character character = _dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id);
            CharacterInventory inventory = character.Inventory;

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = $"Inventory of **{character.Name}**",
                Description = $"Money: {inventory.GetMoney()}",
                Color = Config.Instance.Color,
            };

            StringBuilder str = new StringBuilder();
            str.AppendLine("```ml");

            for (int i = 0 ; i < inventory.GetItems().Count; i++)
            {
                var go = inventory.GetItems()[i];
                str.AppendLine($"'{go.Name}' -  Quantity: {go.Quantity} - Id: {i+1}");
            }
                

            if(inventory.GetItems().Count == 0)
                str.AppendLine("'Empty'");
            str.AppendLine("```");


            embed.AddField("Inventory", str.ToString());
            embed.AddField("Equipment", "```Hand: ```");


            DiscordDmChannel dm = await ctx.Member.CreateDmChannelAsync();
            await dm.SendMessageAsync(embed: embed);


        }
    }
}
