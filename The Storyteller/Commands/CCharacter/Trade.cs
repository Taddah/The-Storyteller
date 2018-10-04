using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Entities.Tools;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MGameObject;

namespace The_Storyteller.Commands.CCharacter
{
    /// <summary>
    /// Permet d'échanger des items entre 2 joueurs par PM
    /// </summary>
    class Trade
    {
        private readonly Dependencies dep;

        public Trade(Dependencies d)
        {
            dep = d;
        }


        [Command("trade")]
        public async Task TradeCommand(CommandContext ctx, params string[] name)
        {
            //Vérification de base character + guild + name fourni
            if (!dep.Entities.Characters.IsPresent(ctx.User.Id)
                || !dep.Entities.Guilds.IsPresent(ctx.Guild.Id)
                || name.Length == 0)
            {
                return;
            }

            InteractivityModule interactivity = ctx.Client.GetInteractivityModule();
            DiscordMemberConverter dmc = new DiscordMemberConverter();
            bool tradeIsOver = false;

            //Nos deux character
            Character currentCharacter = dep.Entities.Characters.GetCharacterByDiscordId(ctx.User.Id);
            DiscordDmChannel dmCurrentCharacter = await ctx.Member.CreateDmChannelAsync();

            Character otherCharacter;

            //Récupérer ID 2ème character
            string strName = string.Join(" ", name);

            if (dmc.TryConvert(strName, ctx, out DiscordMember member))
            {
                otherCharacter = dep.Entities.Characters.GetCharacterByDiscordId(member.Id);
            }
            else
            {
                otherCharacter = dep.Entities.Characters.GetCharacterByName(strName);
            }
            //Toujours rien ? -> Test par truenae
            if(otherCharacter == null)
            {
                otherCharacter = dep.Entities.Characters.GetCharacterByTrueName(strName);
            }

            //Vérification character2
            if(otherCharacter == null)
            {
                var embedNotFound = dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("errorCharacterNotExist"));
                await ctx.Channel.SendMessageAsync(embed: embedNotFound);
                return;
            }

            //Vérification même location
            if(!currentCharacter.Location.Equals(otherCharacter.Location))
            {
                var embedNotOnSameCase= dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("errorCharacterNotOnSameMapForTrade"));
                await ctx.Channel.SendMessageAsync(embed: embedNotOnSameCase);
                return;
            }

            var embedWaitingAnswer = dep.Embed.CreateBasicEmbed(ctx.User, dep.Dialog.GetString("waitingAnswer", otherCharacter));
            await ctx.Channel.SendMessageAsync(embed: embedWaitingAnswer);


            //Les deux inventaires
            Inventory invC1 = dep.Entities.Inventories.GetInventoryById(currentCharacter.Id);
            Inventory invC2 = dep.Entities.Inventories.GetInventoryById(otherCharacter.Id);

            //Liste des items à échanger
            List<GameObject> itemFromC1 = new List<GameObject>();
            List<GameObject> itemFromC2 = new List<GameObject>();

            var applicantDiscordMember = await ctx.Client.GetUserAsync(otherCharacter.Id);
            DiscordDmChannel dmOtherCharacter = await ctx.Client.CreateDmAsync(applicantDiscordMember);

            var embedProposeTrade = dep.Embed.CreateBasicEmbed(applicantDiscordMember, $"{currentCharacter.Name} want to trade with you ! Type {Config.Instance.Prefix}accept to start the trade.");
            await dmOtherCharacter.SendMessageAsync(embed: embedProposeTrade);

            //1 Attendre que member accepte le trade
            MessageContext msgAccept = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == otherCharacter.Id && xm.Content == $"{Config.Instance.Prefix}accept", TimeSpan.FromMinutes(1));
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
                (xm.Author.Id == currentCharacter.Id && xm.ChannelId == dmCurrentCharacter.Id)
                || (xm.Author.Id == otherCharacter.Id && xm.ChannelId == dmOtherCharacter.Id)
                , TimeSpan.FromSeconds(30));

                if (tradeMsg != null)
                {
                    string msg = tradeMsg.Message.Content;
                    //1 Echange accepté
                    if (msg == $"{Config.Instance.Prefix}confirm")
                    {
                        if (tradeMsg.Message.Author.Id == currentCharacter.Id)
                        {
                            tradeC1Confirmed = true;
                        }
                        else if (tradeMsg.Message.Author.Id == otherCharacter.Id)
                        {
                            tradeC2Confirmed = true;
                        }
                    }
                    // 2 Echangé refusé
                    else if (msg == $"{Config.Instance.Prefix}cancel")
                    {
                        if (tradeMsg.Message.Author.Id == currentCharacter.Id)
                        {
                            tradeC1Confirmed = false;
                            tradeC1Canceled = true;
                        }
                        else if (tradeMsg.Message.Author.Id == otherCharacter.Id)
                        {
                            tradeC2Confirmed = false;
                            tradeC2Canceled = true;
                        }
                    }
                    //3 Ajout d'un item
                    else if (msg.StartsWith($"{Config.Instance.Prefix}add"))
                    {
                        tradeC1Confirmed = false;
                        tradeC2Confirmed = false;
                        //a) Nom de l'objet + quantité
                        string[] objectsToAdd = msg.Split(' ');
                        string itemName = "";

                        //Prendre quantité et viré de l'array si bien présent
                        string strCount = objectsToAdd.Last();
                        if(int.TryParse(strCount, out int itemCount))
                            objectsToAdd = objectsToAdd.Take(objectsToAdd.Count() - 1).ToArray();
                        else
                            itemCount = 1;

                        //Remove commands
                        objectsToAdd = objectsToAdd.Skip(1).ToArray();

                        //reconstruire nom objet
                        for (int i = 0; i < objectsToAdd.Length; i++)
                        {
                            itemName += objectsToAdd[i] + " ";
                        }
                        itemName = itemName.Remove(itemName.Length - 1);
                        
                        //b) on vérifie si le joueur a bien les items en inventaire
                        if (tradeMsg.Message.Author.Id == currentCharacter.Id)
                        {
                            GameObject go = invC1.GetGOAndRemoveFromInventory(itemName, itemCount);
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
                        else if (tradeMsg.Message.Author.Id == otherCharacter.Id)
                        {
                            GameObject go = invC2.GetGOAndRemoveFromInventory(itemName, itemCount);
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
                    else if (msg.StartsWith($"{Config.Instance.Prefix}remove"))
                    {
                        //a) Nom de l'objet + quantité
                        string[] objectsToAdd = msg.Split(' ');
                        string itemName = "";

                        //reconstruire nom objet
                        for (int i = 1; i < objectsToAdd.Length; i++)
                        {
                            itemName += objectsToAdd[i] + " ";
                        }
                        itemName = itemName.Remove(itemName.Length - 1);

                        if (tradeMsg.Message.Author.Id == currentCharacter.Id)
                        {
                            if (itemFromC1.Exists(item => item.Name.ToLower() == itemName.ToLower()))
                            {
                                var objectToRemove = itemFromC1.Single(item => item.Name.ToLower() == itemName.ToLower());
                                itemFromC1.Remove(objectToRemove);
                                invC1.AddItem(objectToRemove);
                            }
                        }
                        else if (tradeMsg.Message.Author.Id == otherCharacter.Id)
                        {
                            if (itemFromC2.Exists(item => item.Name.ToLower() == itemName.ToLower()))
                            {
                                var objectToRemove = itemFromC2.Single(item => item.Name.ToLower() == itemName.ToLower());
                                itemFromC2.Remove(objectToRemove);
                                invC2.AddItem(objectToRemove);
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
                        invC1.AddItems(itemFromC1);
                        invC2.AddItems(itemFromC2);
                        tradeIsOver = true;
                    }

                    if (tradeC1Confirmed && tradeC2Confirmed)
                    {
                        await dmCurrentCharacter.SendMessageAsync("Trade has been made");
                        await dmOtherCharacter.SendMessageAsync("Trade has been made");
                        invC1.AddItems(itemFromC2);
                        invC2.AddItems(itemFromC1);
                        tradeIsOver = true;
                    }
                }
            }

            await msg1.DeleteAsync();
            await msg2.DeleteAsync();
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


            string description = $"**{Config.Instance.Prefix}confirm** to confirm, **{Config.Instance.Prefix}cancel** to cancel, **{Config.Instance.Prefix}add [itemName] [quantity]** to add an item, **{Config.Instance.Prefix}remove [itemName]** to remove an item";

            DiscordEmbedBuilder embed = dep.Embed.CreateDetailledEmbed(title, attributes, description: description, inline: true);

            return embed;
        }
    }
}
