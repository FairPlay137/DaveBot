using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DaveBot.Common.ModuleBehaviors
{
    /// <summary>
    /// First thing to be executed; won't prevent further executions
    /// </summary>
    public interface IPreXExecutor
    {
        Task EarlyExecute(DiscordSocketClient client, IGuild guild, IUserMessage msg);
    }
}
