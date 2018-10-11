using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace DaveBot.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class CannotUseInDMsAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if(context.Channel == null)
                return Task.FromResult(PreconditionResult.FromError(StringResourceHandler.GetTextStatic("err", "unavailableInDMs")));
            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
