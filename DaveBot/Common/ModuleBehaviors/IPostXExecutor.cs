using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DaveBot.Common.ModuleBehaviors
{
    /// <summary>
    /// Description TBD
    /// </summary>
    public interface IPostXExecutor
    {
        Task LateExecute(DiscordSocketClient client, IGuild guild, IUserMessage msg);
    }
}
