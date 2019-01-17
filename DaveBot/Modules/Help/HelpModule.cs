using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DaveBot.Common;
using DaveBot.Services;
using System.Collections.Generic;

namespace DaveBot.Modules
{
    [Name("Help")]
    public class HelpModule : DaveBotTopModuleBase
    {
        private readonly DaveBot _bot;
        private readonly CommandService _cs;
        private readonly IBotConfiguration _config;

        public HelpModule(DaveBot bot)
        {
            _bot = bot;
            _cs = bot.CommandService;
            _config = bot.Configuration;
        }

        [Command("help")]
        [Summary("DMs you some helpful information.")]
        [Alias("h")]
        public async Task HelpCmd()
        {
            var dmchannel = await Context.User.GetOrCreateDMChannelAsync();

            string dmcontent = StringResourceHandler.GetTextStatic("Help", "DMContent",_config.DefaultPrefix,
                (_config.BotName == "DaveBot")? //I did have some compiler flags to switch out the two strings when it was a PublicRelease build, but they didn't work, so I removed them before I committed this change
                StringResourceHandler.GetTextStatic("Help", "introPublic")
                :
                (_config.BotOwnerID == 287675563446108160)?
                StringResourceHandler.GetTextStatic("Help", "intro2", _config.BotName)
                :
                StringResourceHandler.GetTextStatic("Help", "intro", _config.BotName)
                );
            if (_config.BotOwnerID == 0)
                dmcontent += StringResourceHandler.GetTextStatic("Help", "DMContentNoBotOwner");
            else
                dmcontent += StringResourceHandler.GetTextStatic("Help", "DMContentContactBotOwner",Context.Client.GetUser(_config.BotOwnerID).Mention, Context.Client.GetUser(_config.BotOwnerID).Username, Context.Client.GetUser(_config.BotOwnerID).Discriminator);
            await dmchannel.SendMessageAsync(dmcontent);
            if(!Context.IsPrivate)
                await ReplyAsync(StringResourceHandler.GetTextStatic("Help", "DMedHelp"));
        }
        [Command("modules")]
        [Summary("Lists all modules.")]
        public async Task Modules()
        {
            var moduleseb = new EmbedBuilder().WithTitle(StringResourceHandler.GetTextStatic("Help", "modules_header")).WithColor(Color.Orange);
            var moduleList = _cs.Modules;
            
            foreach (var module in moduleList)
                if(!module.IsSubmodule)
                    moduleseb.AddField("» " + module.Name, (module.Commands.Count > 0)?
                    StringResourceHandler.GetTextStatic("Help", "modules_commandcount",module.Commands.Count)
                    :
                    StringResourceHandler.GetTextStatic("Help", "modules_emptymodule"));
            await ReplyAsync(Context.User.Mention, false, moduleseb.Build());
            await ReplyAsync("", false, new EmbedBuilder().WithDescription(StringResourceHandler.GetTextStatic("Help", "modules_moreInfo", _config.DefaultPrefix)).WithColor(Color.Orange).Build());
        }
        [Command("commands")]
        [Summary("Lists all commands in a module.")]
        [Alias("cmds")]
        public async Task Commands([Remainder]string modulename)
        {
            ModuleInfo targetmodule = null;
            foreach (var module in _cs.Modules)
                if (module.Name.ToLower() == modulename.ToLower())
                    targetmodule = module;
            if((targetmodule == null)|| (targetmodule.IsSubmodule))
                throw new CommandUnsuccessfulException(StringResourceHandler.GetTextStatic("err", "nonexistentModule"));
            else
            {
                var commandseb = new EmbedBuilder().WithTitle(StringResourceHandler.GetTextStatic("Help", "commands_header",targetmodule.Name)).WithColor(Color.Orange);
                if (targetmodule.Commands.Count > 0)
                {
                    foreach (var command in targetmodule.Commands)
                        commandseb.AddField("» " + _config.DefaultPrefix + command.Name, command.Summary);
                    
                }
                else
                {
                    if(targetmodule.Submodules.Count == 0)
                        commandseb.WithDescription(StringResourceHandler.GetTextStatic("Help", "commandListEmpty"));
                }
                foreach (var submodule in targetmodule.Submodules)
                {
                    if (submodule.Commands.Count > 0)
                    {
                        foreach (var command in submodule.Commands)
                            commandseb.AddField("» " + _config.DefaultPrefix + command.Name, command.Summary);
                    }
                    else
                    {
                        commandseb.WithDescription(StringResourceHandler.GetTextStatic("Help", "commandListEmpty"));
                    }
                }
                await ReplyAsync(Context.User.Mention, false, commandseb.Build());
            }
        }
        /*[Command("help")]
        [Summary("Shows detailed help for a specific command.")]
        [Alias("h")]
        public Task CommandHelp([Remainder] string commandName)
        {
            return Task.FromResult(ExecuteResult.FromError(CommandError.Unsuccessful, StringResourceHandler.GetTextStatic("err", "unimplementedCommand")));
        }*/
    }
}
