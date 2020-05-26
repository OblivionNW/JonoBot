using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JonoBot.Modules {
    /// <summary>
    /// Basic command module for the bot. 
    /// </summary>
    public abstract class JonoBotModuleBase : ModuleBase<JonoBotCommandContext> {
        public async Task<IUserMessage> Reply(Embed embed = null, RequestOptions Options = null) {
            return await Context.Channel.SendMessageAsync("", false, embed, Options).ConfigureAwait(false);
        }

        public async Task<IUserMessage> Reply(string Message, Embed embed = null, RequestOptions Options = null) {
            return await Context.Channel.SendMessageAsync(Message, false, embed, Options).ConfigureAwait(false);
        }

        public async Task<IUserMessage> Reply(string FileName, string Message = null, RequestOptions Options = null) {
            return await Context.Channel.SendFileAsync(FileName, Message, false, null, Options).ConfigureAwait(false);
        }
    }
}
