using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Models;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MGameObject;
using The_Storyteller.Models.MMap;
using The_Storyteller.Models.MMap.MCase;
using The_Storyteller.Models.MVillage;
using The_Storyteller.Models.MVillage.Buildings;

namespace The_Storyteller.Commands.CVillage
{
    /// <summary>
    /// Création d'un village, le coût est de 500 et devrait être défni autrement qu'en dur ...
    /// bref à faire
    /// </summary>
    class CreateVillage
    {
        private readonly Dependencies dep;

        public CreateVillage(Dependencies d)
        {
            dep = d;
        }

        [Command("createVillage")]
        public async Task CreateVillageCommand(CommandContext ctx)
        {
            //Vérification de base character + guild
            if (!dep.Entities.Characters.IsPresent(ctx.Member.Id)
                || !dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }

            Guild guild = dep.Entities.Guilds.GetGuildById(ctx.Guild.Id);
            Region region = dep.Entities.Map.GetRegionByName(guild.RegionName);
            Character character = dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id);
            

            if (region == null || region.GetVillageId() != ulong.MinValue|| !character.Location.Equals(region.GetCentralCase().Location))
            {
                //Region n'appartient pas à un serveur, a déjà un village ou case pas adaptée, impossible de construire ici
                DiscordEmbedBuilder embedNotPossible = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Dialog.GetString("createVillageNotPossible", character: character, region: region));
                await ctx.RespondAsync(embed: embedNotPossible);
                return;
            }

            Inventory inv = dep.Entities.Inventories.GetInventoryById(character.Id);

            if (inv.GetMoney() < 500)
            {
                //Trop pauvre pour construire un village ..
                DiscordEmbedBuilder embedNoMoney = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Dialog.GetString("createVillageNoMoney", character: character));
                await ctx.RespondAsync(embed: embedNoMoney);
                return;
            }


            ////////Sinon, on peut envisager la construction ... !
            InteractivityModule interactivity = ctx.Client.GetInteractivityModule();
            Village village = new Village();
            //1 Demander le nom
            DiscordEmbedBuilder embedVillageName = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Dialog.GetString("createVillageAskName"));
            await ctx.RespondAsync(embed: embedVillageName);
            bool VillageName = false;
            do
            {
                MessageContext msgTrueName = await interactivity.WaitForMessageAsync(
                    xm => xm.Author.Id == ctx.User.Id && xm.ChannelId == ctx.Channel.Id, TimeSpan.FromMinutes(1));
                if (msgTrueName != null)
                {
                    if (msgTrueName.Message.Content.Length <= 50
                        && !dep.Entities.Villages.IsNameTaken(msgTrueName.Message.Content)
                        && msgTrueName.Message.Content.Length > 2)
                    {
                        village.Name = msgTrueName.Message.Content;
                        village.Name = dep.Dialog.RemoveMarkdown(village.Name);
                        VillageName = true;
                    }
                    else
                    {
                        DiscordEmbedBuilder embedErrorTrueName = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Dialog.GetString("startIntroTrueTaken"));
                        await ctx.RespondAsync(embed: embedErrorTrueName);
                    }
                }
            } while (!VillageName);

            //2 Nom ok, on créer le village de base
            village.Id = ctx.Guild.Id;
            village.RegionName = region.Name;
            village.KingId = character.Id;

            //Coût du village retiré au joueur
            inv.RemoveMoney(500);

            //250 va dans les caisses du villages, le reste servant à la "contructions" des bâtiments de base
            Inventory inventory = new VillageInventory();
            inventory.AddMoney(250);
            inventory.Id = village.Id;
            dep.Entities.Inventories.AddInventory(inventory);


            //Ajout des bâtiment de base
            Castle castle = new Castle()
            {
                Level = 1,
                Name = "Castle",
                ProprietaryId = character.Id
            };
            House house = new House()
            {
                Level = 1,
                Name = "Hut",
                ProprietaryId = character.Id
            };
            village.AddBuilding(castle);
            village.AddBuilding(house);

            //Add the king as inhabitant
            village.AddInhabitant(character);
            character.VillageName = village.Name;
            dep.Entities.Characters.EditCharacter(character);

            //become king
            character.Profession = Profession.King;

            //Village rattaché à la région
            region.SetVillageId(village.Id);

            //Case de la région mise en non valable
            region.GetCentralCase().IsAvailable = false;
            region.SetCentralCase(CaseFactory.BuildCase("village", region.GetCentralCase().Location));

            dep.Entities.Villages.AddVillage(village);

            //Bravo, village créé
            DiscordEmbedBuilder embedVillageCreated = dep.Embed.CreateBasicEmbed(ctx.Member, dep.Dialog.GetString("createVillageDone", character: character, village: village));
            await ctx.RespondAsync(embed: embedVillageCreated);

        }
    }
}
