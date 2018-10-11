using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DaveBot.Common.ModuleBehaviors
{
    /// <summary>
    /// Description TBD
    /// </summary>
    public interface IPostXBlockerExecutor
    {
        Task<bool> TryExecuteLate(DiscordSocketClient client, IGuild guild, IUserMessage msg,
            IMessageChannel channel, IUser user, string moduleName, string commandName);
    }
}
