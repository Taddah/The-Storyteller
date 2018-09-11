using DSharpPlus.Entities;

namespace The_Storyteller.Entities.Tools
{
    /// <summary>
    /// Classe de génération d'embed simplifié
    /// </summary>
    internal class EmbedGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberTarget">Pour indiquer à qui est destiné l'embed</param>
        /// <param name="mainText">Obligatoire. Texte principal de l'embed</param>
        /// <param name="optionalText">Optionnel. Texte secondaire en footer</param>
        /// <param name="withPicture">Optionnel. Affichage ou non de l'image du storyteller</param>
        /// <returns></returns>
        public DiscordEmbedBuilder CreateEmbed(DiscordMember memberTarget, string mainText, string optionalText = "", bool withPicture = false)
        {
            var embed = new DiscordEmbedBuilder
            {
                Description = mainText,
                Color = Config.Instance.Color,
                Title = memberTarget.DisplayName
            };

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