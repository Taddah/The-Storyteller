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
using The_Storyteller.Models.MGameObject;

namespace The_Storyteller.Commands.CCharacter
{
    class Trade
    {
        private readonly Dependencies dep;

        public Trade(Dependencies d)
        {
            dep = d;
        }


        [Command("trade")]
        public async Task TradeCommand(CommandContext ctx, DiscordMember member)
        {
            //Vérification de base character + guild
            if (!dep.Entities.Characters.IsPresent(ctx.Member.Id)
                || !dep.Entities.Characters.IsPresent(member.Id)
                || !dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }

            InteractivityModule interactivity = ctx.Client.GetInteractivityModule();
            bool tradeIsOver = false;

            //Nos deux character
            Models.MCharacter.Character currentCharacter = dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id);
            Models.MCharacter.Character otherCharacter = dep.Entities.Characters.GetCharacterByDiscordId(member.Id);

            //Liste des items à échanger
            List<GameObject> itemFromC1 = new List<GameObject>();
            List<GameObject> itemFromC2 = new List<GameObject>();

            //Ouverture d'un DM avec chacun => privacy trade !
            DiscordDmChannel dmCurrentCharacter = await ctx.Member.CreateDmChannelAsync();
            DiscordDmChannel dmOtherCharacter = await member.CreateDmChannelAsync();


            await ctx.RespondAsync($"{currentCharacter.Name} want to trade with you {member.Mention} ! Type {Config.Instance.Prefix}accept to start the trade.");

            //1 Attendre que member accepte le trade
            MessageContext msgAccept = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == member.Id && xm.Content == $"{Config.Instance.Prefix}accept", TimeSpan.FromMinutes(1));
            if (msgAccept != null)
            {
                if (msgAccept.Message.Content != $"{Config.Instance.Prefix}accept")
                {
                    return;
                }
            }

            //Accepté, on montre l'embed d'échange en MP aux 2
            DiscordEmbedBuilder tradeEmbed = GetTradeEmbed(ctx, currentCharacter.Name, otherCharacter.Name, itemFromC1, itemFromC2);

            //Message d'aide


            DiscordMessage msg1 = await dmCurrentCharacter.SendMessageAsync(embed: tradeEmbed);
            DiscordMessage msg2 = await dmOtherCharacter.SendMessageAsync(embed: tradeEmbed);

            bool tradeC1Confirmed = false;
            bool tradeC2Confirmed = false;
            bool tradeC1Canceled = false;
            bool tradeC2Canceled = false;

            while (!tradeIsOver)
            {

                //Attendre message d'un des deux dans le DM
                MessageContext tradeMsg = await interactivity.WaitForMessageAsync(xm =>
                (xm.Author.Id == currentCharacter.DiscordID && xm.ChannelId == dmCurrentCharacter.Id)
                || (xm.Author.Id == otherCharacter.DiscordID && xm.ChannelId == dmOtherCharacter.Id)
                , TimeSpan.FromSeconds(30));

                if (tradeMsg != null)
                {
                    string msg = tradeMsg.Message.Content;
                    //1 Echange accepté
                    if (msg == $"{Config.Instance.Prefix}confirm")
                    {
                        if (tradeMsg.Message.Author.Id == currentCharacter.DiscordID)
                        {
                            tradeC1Confirmed = true;
                        }
                        else if (tradeMsg.Message.Author.Id == otherCharacter.DiscordID)
                        {
                            tradeC2Confirmed = true;
                        }
                    }
                    // 2 Echangé refusé
                    else if (msg == $"{Config.Instance.Prefix}cancel")
                    {
                        if (tradeMsg.Message.Author.Id == currentCharacter.DiscordID)
                        {
                            tradeC1Confirmed = false;
                            tradeC1Canceled = true;
                        }
                        else if (tradeMsg.Message.Author.Id == otherCharacter.DiscordID)
                        {
                            tradeC2Confirmed = false;
                            tradeC2Canceled = true;
                        }
                    }
                    //3 Ajout d'un item
                    else if (msg.StartsWith($"{Config.Instance.Prefix}add"))
                    {
                        //a) Nom de l'objet + quantité
                        string[] objectsToAdd = msg.Split(' ');
                        string itemName = objectsToAdd[1];
                        int.TryParse(objectsToAdd[2], out int itemCount);

                        if (itemCount > 0)
                        {
                            //b) on vérifie si le joueur a bien les items en inventaire
                            if (tradeMsg.Message.Author.Id == currentCharacter.DiscordID)
                            {
                                GameObject go = currentCharacter.Inventory.GetGOAndRemoveFromInventory(itemName, itemCount);
                                if (go != null)
                                {
                                    //Si item existe déjà, on additionne les quantité
                                    if (itemFromC1.Exists(item => item.Name == go.Name))
                                    {
                                        itemFromC1.Single(item => item.Name == go.Name).Quantity += go.Quantity;
                                    }
                                    else
                                    {
                                        itemFromC1.Add(go);
                                    }
                                }
                            }
                            else if (tradeMsg.Message.Author.Id == otherCharacter.DiscordID)
                            {
                                GameObject go = otherCharacter.Inventory.GetGOAndRemoveFromInventory(itemName, itemCount);
                                if (go != null)
                                {
                                    //Si item existe déjà, on additionne les quantité
                                    if (itemFromC2.Exists(item => item.Name == go.Name))
                                    {
                                        itemFromC2.Single(item => item.Name == go.Name).Quantity += go.Quantity;
                                    }
                                    else
                                    {
                                        itemFromC2.Add(go);
                                    }
                                }
                            }
                        }
                    }

                    //Fin, on actualise les messages et on édite
                    tradeEmbed = GetTradeEmbed(ctx, currentCharacter.Name, otherCharacter.Name, itemFromC1, itemFromC2, tradeC1Confirmed, tradeC2Confirmed, tradeC1Canceled, tradeC2Canceled);

                    await msg1.ModifyAsync(embed: tradeEmbed);
                    await msg2.ModifyAsync(embed: tradeEmbed);


                    if (tradeC1Canceled || tradeC2Canceled)
                    {
                        await dmCurrentCharacter.SendMessageAsync("Trade has been canceled");
                        await dmOtherCharacter.SendMessageAsync("Trade has been canceled");
                        currentCharacter.Inventory.AddItems(itemFromC1);
                        otherCharacter.Inventory.AddItems(itemFromC2);
                        tradeIsOver = true;
                    }

                    if (tradeC1Confirmed && tradeC2Confirmed)
                    {
                        currentCharacter.Inventory.AddItems(itemFromC2);
                        otherCharacter.Inventory.AddItems(itemFromC1);
                    }


                }

            }
            //On attend les interactions tant que c'est en cours
        }

        private DiscordEmbedBuilder GetTradeEmbed(CommandContext ctx, string name1, string name2, List<GameObject> itemFromP1, List<GameObject> itemFromP2, bool tradeP1Confirmed = false, bool tradeP2Confirmed = false, bool tradeP1Canceled = false, bool tradeP2Canceled = false)
        {
            DiscordEmoji emojiConfirmed = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
            DiscordEmoji emojiCanceled = DiscordEmoji.FromName(ctx.Client, ":x:");

            string title = $"Trade between **{name1}** and **{name2}**";

            List<string> itemsP1 = new List<string>();
            foreach (GameObject go in itemFromP1)
            {
                itemsP1.Add($"{go.Name} - Quantity: {go.Quantity}");
            }

            name1 = $"**{name1}**";
            if (tradeP1Confirmed)
            {
                name1 += "   " + emojiConfirmed;
            }
            else if (tradeP1Canceled)
            {
                name1 += "   " + emojiCanceled;
            }


            //2 character2
            List<string> itemsP2 = new List<string>();
            foreach (GameObject go in itemFromP2)
            {
                itemsP2.Add($"{go.Name} - Quantity: {go.Quantity}");
            }

            name2 = $"**{name2}**";
            if (tradeP2Confirmed)
            {
                name2 += "   " + emojiConfirmed;
            }
            else if (tradeP2Canceled)
            {
                name2 += "   " + emojiCanceled;
            }


            List<CustomEmbedField> attributes = new List<CustomEmbedField>
            {
                //1 items from P1
                new CustomEmbedField()
                {
                    Name = name1,
                    Attributes = itemsP1
                },
                //2 items from P2
                new CustomEmbedField()
                {
                    Name =name2,
                    Attributes = itemsP2
                }
            };

            
            string description = $"**{Config.Instance.Prefix}confirm** to confirm, **{Config.Instance.Prefix}cancel** to cancel, **{Config.Instance.Prefix}add [itemName] [quantity]** to add an item to trade";

            DiscordEmbedBuilder embed = dep.Embed.CreateDetailledEmbed(title, attributes, description: description, inline: true);

            return embed;
        }
    }
}
