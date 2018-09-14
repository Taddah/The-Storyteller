using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Text;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Entities.Tools;

namespace The_Storyteller.Commands.CCharacter
{
    class Trade
    {
        private readonly Dependencies _dep;

        public Trade(Dependencies d)
        {
            _dep = d;
        }


        [Command("trade")]
        public async Task TradeCommand(CommandContext ctx, DiscordMember member)
        {
            //Vérification de base character + guild
            if (!_dep.Entities.Characters.IsPresent(ctx.Member.Id)
                || !_dep.Entities.Characters.IsPresent(member.Id)
                || !_dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }

            InteractivityModule interactivity = ctx.Client.GetInteractivityModule();
            var currentCharacter = _dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id);
            var otherCharacter = _dep.Entities.Characters.GetCharacterByDiscordId(member.Id);
            await ctx.RespondAsync($"{currentCharacter.Name} want to trade with you {member.Mention} ! Type {Config.Instance.Prefix}accept to start the trade.");

            //1 Attendre que member accepte le trade
            MessageContext msgAccept = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == member.Id && xm.Content == $"{Config.Instance.Prefix}accept", TimeSpan.FromMinutes(1));
            if (msgAccept != null)
            {
                if (msgAccept.Message.Content != $"{Config.Instance.Prefix}accept") return;
            }

            //Accepté, on montre l'embed d'échange
            var tradeEmbed = GetTradeEmbed(currentCharacter.Name, otherCharacter.Name);
            var msg = ctx.RespondAsync(embed: tradeEmbed);
        }

        private DiscordEmbedBuilder GetTradeEmbed(string name1, string name2)
        {
            var embed = new DiscordEmbedBuilder()
            {
                Color = Config.Instance.Color,
                Title = $"Trade between **{name1}** and **{name2}**"
            };

            //1 character1
            StringBuilder str1 = new StringBuilder("```");
            str1.AppendLine("machin : 1");
            str1.AppendLine("```");

            embed.AddField($"**{name1}**", str1.ToString());

            //2 character2
            StringBuilder str2 = new StringBuilder("```");
            str1.AppendLine("truc : 22");
            str1.AppendLine("```");

            embed.AddField($"**{name2}**", str2.ToString());

            return embed;
        }
    }
}
