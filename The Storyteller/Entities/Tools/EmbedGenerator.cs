using DSharpPlus.Entities;

namespace The_Storyteller.Entities.Tools
{
    internal class EmbedGenerator
    {
        public DiscordEmbedBuilder createEmbed(string mainText, string optionalText = "", bool withPicture = false)
        {
            var embed = new DiscordEmbedBuilder();
            embed.Description = mainText;
            embed.Color = Config.Instance.Color;

            if (optionalText.Length > 0)
                embed.Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = optionalText
                };

            if (withPicture) embed.ImageUrl = "https://s33.postimg.cc/h0wkmhnm7/153046781869422717_1.png";

            return embed;
        }
    }
}