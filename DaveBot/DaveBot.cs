using System;
using System.Threading.Tasks;
using NLog;
using System.IO;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NLog.Config;
using NLog.Targets;
using System.Reflection;
using DaveBot.Services;
using Discord.Net.Providers.WS4Net;
using DaveBot.Services.Impl;
using System.Diagnostics;
using DaveBot.Common;

namespace DaveBot
{
    public class DaveBot : IDisposable
    {
        private readonly Logger _log;

        public DiscordShardedClient Client { get; }
        public CommandService CommandService { get; }

        //public TaskCompletionSource<bool> Ready { get; private set; } = new TaskCompletionSource<bool>();
        public TaskCompletionSource<bool> Ready { get; private set; } = new(); //C# 8 compliant syntax

        public IDaveBotServiceProvider Services { get; private set; }

        public BotConfiguration Configuration { get; }

        public DateTime StartTime = DateTime.Now; //An easy way to calculate the startup time
        public DateTime ConnectedAtTime;

        private bool disposed = false;

        public DaveBot()
        {
            SetupLogger();
            _log = LogManager.GetCurrentClassLogger();
            SloppyElevatedPermissionCheck();

            _log.Info($"DaveBot v{GetType().Assembly.GetName().Version} is initializing; please wait...");

#if PUBLIC_BUILD
            _log.Info(" == PUBLIC BUILD ==");
#endif

            Configuration = new BotConfiguration();
            Client = new DiscordShardedClient(new DiscordSocketConfig()
            {
                WebSocketProvider = WS4NetProvider.Instance,
                LogLevel = LogSeverity.Info,
                ConnectionTimeout = int.MaxValue,
                MessageCacheSize = 10,
                AlwaysDownloadUsers = false,
                TotalShards = Configuration.TotalShards
            });
            CommandService = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Sync
            });

            Client.Log += Log; //Route Discord.NET logs to the console
        }

        private void AddServices()
        {
            Services = new DaveBotServiceProvider.ServiceProviderBuilder()
                .AddManual(this)
                .AddManual<IBotConfiguration>(Configuration)
                .AddManual(Client)
                .AddManual(CommandService)
                .LoadFrom(Assembly.GetEntryAssembly())
                .Build();

            var commandHandler = Services.GetService<CommandHandler>();
            commandHandler.AddServices(Services);
        }

        private async Task LoginAsync(string token)
        {
            var clientReady = new TaskCompletionSource<bool>();

            Task SetClientReady(DiscordSocketClient shardClient)
            {
                var _ = Task.Run(async () =>
                {
                    clientReady.TrySetResult(true);
                    try
                    {
                        foreach (var chan in await shardClient.GetDMChannelsAsync())
                        {
                            await chan.CloseAsync().ConfigureAwait(false);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                });
                return Task.CompletedTask;
            }

            //connect
            await Client.LoginAsync(TokenType.Bot, token).ConfigureAwait(false);
            await Client.StartAsync().ConfigureAwait(false);
            Client.ShardReady += SetClientReady;
            await clientReady.Task.ConfigureAwait(false);
            Client.ShardReady -= SetClientReady;
            Client.ShardReady += ShardReady;
            Client.JoinedGuild += GuildJoin;
            Client.LeftGuild += GuildLeave;
            Client.ShardConnected += ShardConnect;
            Client.ShardDisconnected += ShardDisconnect;
        }

        public async Task StartAsync(params string[] args)
        {
#if !PUBLIC_BUILD
            _log.Info("Initialization complete. Starting up...");
#else
            _log.Info("Initialization complete. Waiting for 30 seconds before executing startup routines...");
            await Task.Delay(30000);
            _log.Info("30 seconds have elapsed. Starting up...");
#endif

            var sw = Stopwatch.StartNew();

            await LoginAsync(Configuration.BotToken).ConfigureAwait(false);

            await Client.SetStatusAsync(UserStatus.DoNotDisturb);
            await Client.SetGameAsync(StringResourceHandler.GetTextStatic("generic", "loadingPlayingStatus"));

            _log.Info("Loading services...");
            AddServices();

            sw.Stop();
            _log.Info($"Connected in {sw.Elapsed.TotalSeconds:F4} seconds");

            var commandHandler = Services.GetService<CommandHandler>();
            var commandService = Services.GetService<CommandService>();

            await commandHandler.StartHandling().ConfigureAwait(false);

            var _ = await commandService.AddModulesAsync(GetType().GetTypeInfo().Assembly, Services);

            await Client.SetStatusAsync(UserStatus.Online);
            await Client.SetGameAsync(Configuration.DefaultPlayingString);

            Ready.TrySetResult(true);
            
            ConnectedAtTime = DateTime.Now;
#if PUBLIC_BUILD
            var secsOffset = 30;
#else
            var secsOffset = 0;
#endif
            _log.Info($"It took {new TimeSpan(ConnectedAtTime.Ticks - StartTime.Ticks).TotalSeconds - secsOffset} second(s) to boot up.");
        }

        public async Task StartAndBlockAsync(params string[] args)
        {
            await StartAsync(args).ConfigureAwait(false);
            await Task.Delay(-1).ConfigureAwait(false);
        }

        private static void SetupLogger()
        {
            var logConfig = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget()
            {
                Layout = @"${date:format=HH\:mm\:ss} [${logger}] ${message}"
            };
            logConfig.AddTarget("Console", consoleTarget);

            logConfig.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));

            LogManager.Configuration = logConfig;
        }

        private Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Info:
                    _log.Info(msg.Source + " | " + msg.Message);
                    break;
                case LogSeverity.Error:
                    _log.Error(msg.Source + " | " + msg.Message);
                    break;
                case LogSeverity.Critical:
                    _log.Fatal(msg.Source + " | " + msg.Message);
                    break;
                case LogSeverity.Debug:
                    _log.Debug(msg.Source + " | " + msg.Message);
                    break;
                case LogSeverity.Verbose:
                    _log.Info(msg.Source + " | " + msg.Message);
                    break;
                case LogSeverity.Warning:
                    _log.Warn(msg.Source + " | " + msg.Message);
                    break;
                default: //Hopefully this'll never happen
                    _log.Info(msg.Source + " | " + msg.Message + $" (INVALID SEVERITY LEVEL {msg.Severity.ToString()})");
                    break;
            }

            if (msg.Exception != null) //If there's an exception, output it to the console.
                _log.Warn(msg.Exception);

            return Task.CompletedTask;
        }

        private void SloppyElevatedPermissionCheck()
        {
            try
            {
                File.WriteAllText("testfile", "This is a test file to check to see if DaveBot can save or not. This file should have been deleted as part of the normal startup process. If it hasn't, please delete this file.");
                File.Delete("testfile");
            }
            catch
            {
                _log.Error("DaveBot cannot start, either due to not having permission to save in the program folder, or there being insufficient disk space.\n"+
                    "Please make sure DaveBot is running as an administrator, and that you have at least *some* free space on the drive DaveBot is on.");
                Console.ReadKey();
                Environment.Exit(2);
            }
        }

        private Task GuildJoin(SocketGuild guild)
        {
            _log.Info($"Joined guild: {guild.Name} ({guild.Id})");
            _log.Info($" {guild.TextChannels.Count} Text Channel(s)");
            _log.Info($" {guild.VoiceChannels.Count} Voice Channel(s)");
            _log.Info($" {guild.Users.Count} user(s)");
            _log.Info($" Owner: @{guild.Owner.Username}#{guild.Owner.Discriminator} ({guild.OwnerId})");
            _log.Info($" Created {guild.CreatedAt}");
            return Task.CompletedTask;
        }
        private Task GuildLeave(SocketGuild guild)
        {
            _log.Info($"Left guild: {guild.Name} ({guild.Id})");
            return Task.CompletedTask;
        }
        private Task ShardConnect(DiscordSocketClient client)
        {
            _log.Info($"Shard #{client.ShardId} has connected!");
            return Task.CompletedTask;
        }
        private Task ShardDisconnect(Exception error, DiscordSocketClient client)
        {
            _log.Info($"Shard #{client.ShardId} has lost connection!");
            return Task.CompletedTask;
        }

        private async Task ShardReady(DiscordSocketClient client)
        {
            //_log.Info($"Shard #{client.ShardId} is ready!");
            int recShard = await Client.GetRecommendedShardCountAsync();
            if (Configuration.TotalShards != recShard)
                _log.Warn($"It is recommended that you use {recShard} shard(s) instead of {Configuration.TotalShards}. "
                    + $"Please use \"{Configuration.DefaultPrefix}setshards {recShard}\" to change config.json accordingly.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                Client.Dispose();
                disposed = true;
                if (disposing)
                    GC.SuppressFinalize(this);
            }
        }

        public void Dispose()
        {
            if(!disposed)
                Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~DaveBot()
        {
            Dispose(false);
        }
    }
}
