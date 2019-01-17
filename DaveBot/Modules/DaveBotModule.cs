using Discord.Commands;
using DaveBot.Services;
using DaveBot.Common;

namespace DaveBot.Common
{
    public abstract class DaveBotTopModuleBase : ModuleBase<SocketCommandContext>
    {
        protected DaveBotTopModuleBase(bool isTopLevelModule = true) { }

        public CommandHandler _cmdHandler { get; set; }
    }
    public abstract class DaveBotTopModuleBase<T> : DaveBotTopModuleBase where T : IDaveBotService
    {
        public T _service { get; set; }
        protected DaveBotTopModuleBase(bool isTopLevelModule = true) : base(isTopLevelModule) { }
    }
    public abstract class DaveBotSubModuleBase : DaveBotTopModuleBase
    {
        protected DaveBotSubModuleBase() : base(false) { }
    }
    public abstract class DaveBotSubModuleBase<T> : DaveBotTopModuleBase<T> where T : IDaveBotService
    {
        protected DaveBotSubModuleBase() : base(false) { }
    }
}
