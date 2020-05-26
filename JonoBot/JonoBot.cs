using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JonoBot.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace JonoBot {
    public class JonoBot {
        // The bots Discord client.
        private DiscordSocketClient Client;

        // Services provided by this bot.
        private IServiceProvider ServiceProvider;
        private CommandService CommandService;

        // Start up the bot.
        public static void Main(string[] args) => new JonoBot().Start().GetAwaiter().GetResult();

        /// <summary>
        /// Starts up the bot and establishes a connection to Discord.
        /// </summary>
        private async Task Start() {
            // Initialize the settings for the bot from the config file.
            // TODO: Take this path as a command line argument
            JonoBotSettings.InitializeSettings(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.conf"));

            // Create the bots client.
            Client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = LogSeverity.Info
            });

            // Update client status
            await Client.SetGameAsync(JonoBotSettings.COMMAND_PREFIX + "help");

            CommandService = new CommandService();

            // Create the service provider.
            ServiceProvider = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(CommandService)
                .BuildServiceProvider();

            // Setup the bots command structure.
            await SetupCommands();
            SetupEventListeners();

            // Setup the bots logger.
            Client.Log += Log;
            CommandService.Log += Log;

            // Login to Discord and start the bot.
            await Client.LoginAsync(JonoBotSettings.TOKEN_TYPE, JonoBotSettings.BOT_TOKEN);
            await Client.StartAsync();

            // Block the thread so the bot doesn't terminate.
            await Task.Delay(-1);
        }

        /// <summary>
        /// Runs the bots initial command setup.
        /// </summary>
        private async Task SetupCommands() {
            // Add the command handler and modules.
            Client.MessageReceived += HandleCommand;

            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceProvider);
        }

        /// <summary>
        /// Sets up event listeners.
        /// </summary>
        public void SetupEventListeners() {
            Client.MessageReceived += FuckMaguire;
        }

        /// <summary>
        /// Kicks the man from voice when he tries to play a song.
        /// </summary>
        public async Task FuckMaguire(SocketMessage Message) {
            // Make sure the user is Slabhead/Maguire#1343
            if (Message.Author.Id != 391355602674384896) {
                return;
            }

            // Make sure it's a song being played or one being skipped
            if (!Message.Content.StartsWith("-play") && !Message.Content.StartsWith("-skip")) {
                return;
            }

            // Kick him.
            SocketGuildUser Maguire = (SocketGuildUser)Message.Author;
            await Maguire.ModifyAsync(User => {
                User.Channel = null;
            });

            // Skip the song if a song was added.
            if (Message.Content.StartsWith("-play")) {
                await Message.Channel.SendMessageAsync(StringUtil.ReplaceFirst(Message.Content, "play", "remove"));
            }
        }

        /// <summary>
        /// Handles any possible command occurences in Discord chat rooms.
        /// </summary>
        private async Task HandleCommand(SocketMessage Message) {
            // Cast the message to a user message, if it isn't then don't process it.
            SocketUserMessage UserMessage = Message as SocketUserMessage;
            if (UserMessage == null) {
                return;
            }

            // If our command prefix wasnt used, or the bot wasnt tagged in the message, dont process it. 
            // If its a command, store where the actual command starts.
            int CommandStart = 0;
            if (!(UserMessage.HasCharPrefix(JonoBotSettings.COMMAND_PREFIX, ref CommandStart)
                || UserMessage.HasMentionPrefix(Client.CurrentUser, ref CommandStart))) {
                return;
            }

            // Create the command context and send it to the service for execution.
            JonoBotCommandContext CommandContext = new JonoBotCommandContext(Client, UserMessage);
            IResult CommandResult = await CommandService.ExecuteAsync(CommandContext, CommandStart, ServiceProvider);

            // If there was an error during command execution, send the error to chat.
            if (!CommandResult.IsSuccess) {
                EmbedBuilder ErrorEmbed = new EmbedBuilder();
                ErrorEmbed.WithColor(Color.Red);
                ErrorEmbed.WithDescription("Looks like I couldn't do that. Make sure the command exists and your using it correctly, look at the %help page." +
                    Environment.NewLine + "Error: " + CommandResult.ErrorReason);

                await CommandContext.Channel.SendMessageAsync("", false, ErrorEmbed.Build(), null).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Logs a message to the bots console.
        /// </summary>
        /// <param name="Message">The message.</param>
        private Task Log(LogMessage Message) {
            if (Message.Exception is CommandException CmdException) {
                Console.WriteLine($"{CmdException.Context.User} failed to execute '{CmdException.Command.Name}' in {CmdException.Context.Channel}.");
                Console.WriteLine("[JonoBot Error] {0}", CmdException.ToString());
            } else {
                Console.WriteLine("[JonoBot Log] {0}", Message.ToString());
            }

            return Task.CompletedTask;
        }
    }
}
