using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace JonoBot.Modules {
    /// <summary>
    /// Basic context given to all command processors for the bot.
    /// </summary>
    public class JonoBotCommandContext : ICommandContext {
        public DiscordSocketClient Client { get; }

        public SocketGuild Guild { get; }
        public SocketUser User { get; }
        public ISocketMessageChannel Channel { get; }
        public SocketUserMessage Message { get; }

        public bool IsPrivate => Channel is IPrivateChannel;

        public JonoBotCommandContext(DiscordSocketClient Client, SocketUserMessage Message, SocketUser User = null) {
            this.Client = Client;
            this.Guild = (Message.Channel as SocketGuildChannel)?.Guild;
            this.Channel = Message.Channel;
            this.User = User ?? Message.Author;
            this.Message = Message;
        }

        IDiscordClient ICommandContext.Client => Client;
        IGuild ICommandContext.Guild => Guild;
        IMessageChannel ICommandContext.Channel => Channel;
        IUser ICommandContext.User => User;
        IUserMessage ICommandContext.Message => Message;
    }
}
