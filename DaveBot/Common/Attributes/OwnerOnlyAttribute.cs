using System;
using System.Threading.Tasks;
using Discord.Commands;
using DaveBot.Services;

namespace DaveBot.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class OwnerOnlyAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var config = (IBotConfiguration)services.GetService(typeof(IBotConfiguration));
            if (config.BotOwnerID == 0)
            {
                return Task.FromResult(PreconditionResult.FromError(StringResourceHandler.GetTextStatic("err", "noBotOwner")));
            }
            else
            {
                if (config.BotOwnerID != context.Message.Author.Id)
                {
                    return Task.FromResult(PreconditionResult.FromError(StringResourceHandler.GetTextStatic("err", "notBotOwner")));
                }
                else
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
            }
        }
    }
}
