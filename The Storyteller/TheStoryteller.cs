using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using The_Storyteller.Entities;
using The_Storyteller.Entities.Tools;

namespace The_Storyteller
{
    public class TheStoryteller : IDisposable
    {
        private readonly DiscordClient _client;
        private readonly CancellationTokenSource _cts;
        private readonly EmbedGenerator _embed;

        private readonly EntityManager _entityManager;
        private readonly Dialog _res;
        private CommandsNextModule _cnext;
        private InteractivityModule _interactivity;

        public TheStoryteller()
        {
            if (!File.Exists("config.json"))
            {
                Config.Instance.SaveToFile("config.json");
                Console.WriteLine("config file is empty");
                Environment.Exit(0);
            }

            Config.Instance.LoadFromFile("config.json");
            _client = new DiscordClient(new DiscordConfiguration
            {
                AutoReconnect = true,
                EnableCompression = true,
                LogLevel = LogLevel.Debug,
                Token = Config.Instance.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true
            });

            _interactivity = _client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = TimeoutBehaviour.Default,
                PaginationTimeout = TimeSpan.FromSeconds(30),
                Timeout = TimeSpan.FromSeconds(30)
            });

            _cts = new CancellationTokenSource();
            _res = new Dialog("textResources.json");
            _embed = new EmbedGenerator();

            _entityManager = new EntityManager();

            //Création et ajout des dépendances accessibles depuis toutes les commandes 
            //(voir les exemples)
            DependencyCollection dep = null;
            using (DependencyCollectionBuilder d = new DependencyCollectionBuilder())
            {
                d.AddInstance(new Dependencies
                {
                    Interactivity = _interactivity,
                    Cts = _cts,
                    Dialog = _res,
                    Embed = _embed,
                    Entities = _entityManager
                });
                dep = d.Build();
            }

            _cnext = _client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = false,
                EnableDefaultHelp = false,
                EnableDms = true,
                EnableMentionPrefix = true,
                StringPrefix = Config.Instance.Prefix,
                IgnoreExtraArguments = true,
                Dependencies = dep
            });
            

            /////////CHARACTER COMMANDS
            _cnext.RegisterCommands<Commands.CCharacter.CharacterInfo>();
            _cnext.RegisterCommands<Commands.CCharacter.Move>();
            _cnext.RegisterCommands<Commands.CCharacter.Start>();
            _cnext.RegisterCommands<Commands.CCharacter.ShowInventory>();
            _cnext.RegisterCommands<Commands.CCharacter.Trade>();
            _cnext.RegisterCommands<Commands.CCharacter.JoinVillage>();


            /////////GUILD COMMANDS
            _cnext.RegisterCommands<Commands.CGuild.RegisterGuild>();

            ////////MAP COMMANDS
            _cnext.RegisterCommands<Commands.CMap.RegionInfo>();
            _cnext.RegisterCommands<Commands.CMap.CaseInfo>();


            ////////VILLAGE COMMANDS
            _cnext.RegisterCommands<Commands.CVillage.CreateVillage>();
            _cnext.RegisterCommands<Commands.CVillage.VillageInfo>();


            _client.Ready += OnReadyAsync;
            _client.GuildCreated += OnGuildCreated;
        }

        public void Dispose()
        {
            _client.Dispose();
            _interactivity = null;
            _cnext = null;
        }

        public async Task RunAsync()
        {
            await _client.ConnectAsync();
            await WaitForCancellationAsync();
        }

        private async Task WaitForCancellationAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                await Task.Delay(500);
            }
        }

        private async Task OnReadyAsync(ReadyEventArgs e)
        {
            await Task.Yield();
        }

        private async Task OnGuildCreated(GuildCreateEventArgs e)
        {
            await Task.Yield();
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "The Storyteller", $"New Guild: {e.Guild.Name}",
                DateTime.Now);

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Description = _res.GetString("introduction"),
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = _res.GetString("introductionTypeGen")
                },
                ImageUrl = "https://s33.postimg.cc/h0wkmhnm7/153046781869422717_1.png",
                Color = Config.Instance.Color
            };

            await e.Guild.GetDefaultChannel().SendMessageAsync(embed: embed);
        }
    }
}