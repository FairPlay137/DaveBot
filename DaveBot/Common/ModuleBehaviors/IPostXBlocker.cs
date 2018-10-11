using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DaveBot.Common.ModuleBehaviors
{
    /// <summary>
    /// Description TBD
    /// </summary>
    public interface IPostXBlocker
    {
        Task<bool> TryBlockLate(IUserMessage msg, IGuild guild,
            IMessageChannel channel, IUser user, string moduleName, string commandName);
    }
}
