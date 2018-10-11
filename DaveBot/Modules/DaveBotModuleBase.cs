using Discord.Commands;
using DaveBot.Services;

namespace DaveBot.Common
{
    public abstract class DaveBotModuleBase<T> : ModuleBase<SocketCommandContext>
    {
        public CommandHandler _cmdHandler { get; set; }
    }
}
