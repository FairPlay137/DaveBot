using System;
using Discord.Commands;
using DaveBot.Services;
using Discord.Interactions;

namespace DaveBot.Common
{
    [Obsolete("The normal command modules are deprecated. Please use DaveBotSlashTopModuleBase instead.")]
    public abstract class DaveBotTopModuleBase : ModuleBase<SocketCommandContext>
    {
        protected DaveBotTopModuleBase(bool isTopLevelModule = true) { }

        public CommandHandler _cmdHandler { get; set; }
    }
    [Obsolete("The normal command modules are deprecated. Please use DaveBotSlashTopModuleBase instead.")]
    public abstract class DaveBotTopModuleBase<T> : DaveBotTopModuleBase where T : IDaveBotService
    {
        public T _service { get; set; }
        protected DaveBotTopModuleBase(bool isTopLevelModule = true) : base(isTopLevelModule) { }
    }
    [Obsolete("The normal command modules are deprecated. Please use DaveBotSlashSubModuleBase instead.")]
    public abstract class DaveBotSubModuleBase : DaveBotTopModuleBase
    {
        protected DaveBotSubModuleBase() : base(false) { }
    }
    [Obsolete("The normal command modules are deprecated. Please use DaveBotSlashSubModuleBase instead.")]
    public abstract class DaveBotSubModuleBase<T> : DaveBotTopModuleBase<T> where T : IDaveBotService
    {
        protected DaveBotSubModuleBase() : base(false) { }
    }

    // As of September 1st, 2022, DaveBot will start using slash commands.

    public abstract class DaveBotSlashTopModuleBase : InteractionModuleBase<SocketInteractionContext>
    {
        protected DaveBotSlashTopModuleBase(bool isTopLevelModule = true) { }

        public InteractionHandler _intHandler { get; set; }
    }
    public abstract class DaveBotSlashTopModuleBase<T> : DaveBotSlashTopModuleBase where T : IDaveBotService
    {
        public T _service { get; set; }
        protected DaveBotSlashTopModuleBase(bool isTopLevelModule = true) : base(isTopLevelModule) { }
    }
    public abstract class DaveBotSlashSubModuleBase : DaveBotSlashTopModuleBase
    {
        protected DaveBotSlashSubModuleBase() : base(false) { }
    }
    public abstract class DaveBotSlashSubModuleBase<T> : DaveBotSlashTopModuleBase<T> where T : IDaveBotService
    {
        protected DaveBotSlashSubModuleBase() : base(false) { }
    }
}
