using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Models;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MMap;
using The_Storyteller.Models.MVillage;
using The_Storyteller.Models.MVillage.Buildings;

namespace The_Storyteller.Commands.CVillage
{
    class CreateVillage
    {
        private readonly Dependencies _dep;

        public CreateVillage(Dependencies d)
        {
            _dep = d;
        }

        [Command("createVillage")]
        public async Task CreateVillageCommand(CommandContext ctx)
        {
            //Vérification de base character + guild
            if (!_dep.Entities.Characters.IsPresent(ctx.Member.Id)
                || !_dep.Entities.Guilds.IsPresent(ctx.Guild.Id))
            {
                return;
            }

            Guild guild = _dep.Entities.Guilds.GetGuildById(ctx.Guild.Id);
            Region region = _dep.Entities.Map.GetRegionByName(guild.RegionName);
            Character character = _dep.Entities.Characters.GetCharacterByDiscordId(ctx.Member.Id);

            if (region == null || region.GetVillageId() != -1 || !character.Location.Equals(region.GetCentralCase().Location))
            {
                //Region n'appartient pas à un serveur, a déjà un village ou case pas adaptée, impossible de construire ici
                DiscordEmbedBuilder embedNotPossible = _dep.Embed.CreateEmbed(ctx.Member, _dep.Resources.GetString("createVillageNotPossible", character: character, region: region));
                await ctx.RespondAsync(embed: embedNotPossible);
                return;
            }

            if (character.Inventory.GetMoney() < 500)
            {
                //Trop pauvre pour construire un village ..
                DiscordEmbedBuilder embedNoMoney = _dep.Embed.CreateEmbed(ctx.Member, _dep.Resources.GetString("createVillageNoMoney", character: character));
                await ctx.RespondAsync(embed: embedNoMoney);
                return;
            }


            ////////Sinon, on peut envisager la construction ... !
            InteractivityModule interactivity = ctx.Client.GetInteractivityModule();
            Village village = new Village();
            //1 Demander le nom
            DiscordEmbedBuilder embedVillageName = _dep.Embed.CreateEmbed(ctx.Member, _dep.Resources.GetString("createVillageAskName"));
            await ctx.RespondAsync(embed: embedVillageName);
            bool VillageName = false;
            do
            {
                MessageContext msgTrueName = await interactivity.WaitForMessageAsync(
                    xm => xm.Author.Id == ctx.User.Id && xm.ChannelId == ctx.Channel.Id, TimeSpan.FromMinutes(1));
                if (msgTrueName != null)
                {
                    if (msgTrueName.Message.Content.Length <= 50
                        && !_dep.Entities.Villages.IsNameTaken(msgTrueName.Message.Content)
                        && msgTrueName.Message.Content.Length > 2)
                    {
                        village.Name = msgTrueName.Message.Content;
                        village.Name = _dep.Resources.RemoveMarkdown(village.Name);
                        VillageName = true;
                    }
                    else
                    {
                        DiscordEmbedBuilder embedErrorTrueName = _dep.Embed.CreateEmbed(ctx.Member, _dep.Resources.GetString("startIntroTrueTaken"));
                        await ctx.RespondAsync(embed: embedErrorTrueName);
                    }
                }
            } while (!VillageName);

            //2 Nom ok, on créer le village de base
            village.Id = _dep.Entities.Villages.GetVillageCount() + 1;
            village.RegionName = region.Name;
            village.MayorId = character.DiscordID;

            //Coût du village retiré au joueur
            character.Inventory.RemoveMoney(500);
            //250 va dans les caisses du villages, le reste servant à la "contructions" des bâtiments de base
            village.Inventory = new VillageInventory();
            village.Inventory.AddMoney(250);

            //Ajout des bâtiment de base
            Castle castle = new Castle()
            {
                Level = 1,
                Name = village.Name + "'s castle",
                ProprietaryId = character.Id
            };
            House house = new House()
            {
                Level = 1,
                Name = "hut",
                ProprietaryId = character.Id
            };
            village.AddBuilding(castle);
            village.AddBuilding(house);

            //Add the mayor as inhabitant
            village.AddInhabitant(character);
            character.VillageName = village.Name;
            _dep.Entities.Characters.EditCharacter(character);

            //Village rattaché à la région
            region.SetVillageId(village.Id);

            _dep.Entities.Villages.AddVillage(village);

            //Bravo, village créé
            DiscordEmbedBuilder embedVillageCreated = _dep.Embed.CreateEmbed(ctx.Member, _dep.Resources.GetString("createVillageDone", character: character, village: village));
            await ctx.RespondAsync(embed: embedVillageCreated);

        }
    }
}
