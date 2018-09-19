using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MMap;
using The_Storyteller.Models.MVillage;

namespace The_Storyteller.Commands.CCharacter
{
    internal class JoinVillage
    {
        private readonly Dependencies dep;

        public JoinVillage(Dependencies d)
        {
            dep = d;
        }

        [Command("joinvillage")]
        public async Task JoinVillageCommand(CommandContext ctx)
        {
            //Vérification de base character + guild
            if (!dep.Entities.Characters.IsPresent(ctx.Member.Id)
                || !dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }

            Character character = dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id);
            Case currentCase = dep.Entities.Map.GetCase(character.Location);

            //1 check existence village + appartenance village
            if (character.VillageName != null || currentCase.VillageId == ulong.MinValue)
            {
                DiscordEmbedBuilder embed = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Dialog.GetString("errorCantJoinVillage"));
                await ctx.RespondAsync(embed: embed);
                return;
            }

            //2 sinon ajout liste attente village
            // + prévenir roi ?
            Village village = dep.Entities.Villages.GetVillageById(currentCase.VillageId);
            
            if(!village.WaitingList.Contains(character.DiscordID))
            {
                village.WaitingList.Add(character.DiscordID);
            }

            DiscordMember king = await ctx.Guild.GetMemberAsync(village.KingId);

            if(king != null)
            {
                DiscordDmChannel dm = await king.CreateDmChannelAsync();
                DiscordEmbedBuilder embed = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Dialog.GetString("joinVillageMPKing"));
                await dm.SendMessageAsync(embed: embed);
            }
        }
    }
}
