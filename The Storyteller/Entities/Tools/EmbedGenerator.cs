using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Text;

namespace The_Storyteller.Entities.Tools
{
    internal class EmbedGenerator
    {
        public DiscordEmbedBuilder CreateBasicEmbed(DiscordMember memberTarget, string mainText, string optionalText = "", bool withPicture = false)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = mainText,
                Color = Config.Instance.Color,
                Title = memberTarget.DisplayName
            };

            if (optionalText.Length > 0)
            {
                embed.Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = optionalText
                };
            }

            if (withPicture)
            {
                embed.ImageUrl = "https://s33.postimg.cc/h0wkmhnm7/153046781869422717_1.png";
            }

            return embed;
        }

        public DiscordEmbedBuilder CreateDetailledEmbed(string title, List<CustomEmbedField> fields, string description = "", string footer = "", bool inline = false)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Description = description,
                Color = Config.Instance.Color,
                Title = title
            };

            if (footer != "")
                embed.Footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text = footer
                };

            foreach(CustomEmbedField field in fields)
            {
                var strBuilder = new StringBuilder();
                strBuilder.AppendLine("```");
                foreach (string attr in field.Attributes)
                    strBuilder.AppendLine(attr);
                strBuilder.AppendLine("```");

                embed.AddField($"{field.Name}", strBuilder.ToString(), inline);
            }

            return embed;
        }
    }

    public class CustomEmbedField
    {
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
    }

}