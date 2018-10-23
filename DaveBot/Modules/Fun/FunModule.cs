using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DaveBot.Common;
using DaveBot.Services;
using DaveBot.Modules.Fun.Common;

namespace DaveBot.Modules
{
    [Name("Fun")]
    public class FunModule : DaveBotModuleBase<SocketCommandContext>
    {
        string[] DefaultEightBallResponses = { "It is certain.", "Without a doubt.", "You may rely on it.", "Most likely.", "Outlook good.",
                                        "Yes.", "Don't count on it.", "My reply is no.", "My sources say no.", "Outlook not so good.",
                                        "Very doubtful.", "Reply hazy; try again.", "Ask again later.", "Better not tell you now.",
                                        "Cannot predict now.", "Concentrate and ask again."};

        private readonly IBotConfiguration _config;

        public FunModule(IBotConfiguration config)
        {
            _config = config;
        }

        [Command("8ball")]
        [Summary("Ask the 8ball a question!")]
        public async Task EightBall([Remainder] [Summary("The question")] string question)
        {
            DaveRNG random = new DaveRNG();
            string answer = (_config.EightBallResponses.Length > 0)?
                _config.EightBallResponses[random.Next(_config.EightBallResponses.Length)]:
                DefaultEightBallResponses[random.Next(DefaultEightBallResponses.Length)];
            await ReplyAsync("", false, new EmbedBuilder()
                .AddField($":question: {StringResourceHandler.GetTextStatic("Fun", "8ball_question")}", question)
                .AddField($":8ball: {StringResourceHandler.GetTextStatic("Fun", "8ball_answer")}", answer)
                .WithColor(Color.Blue)
                .Build());
        }
        [Command("choose")]
        [Summary("Choose an option from a given set")]
        public async Task Choose([Remainder] [Summary("Options (seperate with `;`)")] string options)
        {
            DaveRNG random = new DaveRNG();
            string[] optionsIndiv = options.Split(';');
            string choice = optionsIndiv[random.Next(optionsIndiv.Length)];
            await ReplyAsync("", false, new EmbedBuilder()
                .AddField(":thinking:", choice)
                .WithColor(Color.Blue)
                .Build());
        }
        [Command("ship")]
        [Summary("Command description TBD")]
        public async Task Ship([Remainder] [Summary("Two targets, separated by `;`")] string targets)
        {
            string[] args = targets.Split(';');
            if (args.Length == 2)
            {
                var msg = await ReplyAsync(StringResourceHandler.GetTextStatic("generic", "PleaseWait")).ConfigureAwait(false);
                var tstate = Context.Channel.EnterTypingState();
                int percentage = MatchmakingLogic.CalculateMatchmakingPercentage(args[0], args[1]);
                string commenttext = percentage == 100
                    ? StringResourceHandler.GetTextStatic("Fun", "ship_result_8")
                    : percentage > 85
                    ? StringResourceHandler.GetTextStatic("Fun", "ship_result_7")
                    : percentage > 70
                    ? StringResourceHandler.GetTextStatic("Fun", "ship_result_6")
                    : percentage > 55
                    ? StringResourceHandler.GetTextStatic("Fun", "ship_result_5")
                    : percentage > 40
                    ? StringResourceHandler.GetTextStatic("Fun", "ship_result_4")
                    : percentage > 25
                    ? StringResourceHandler.GetTextStatic("Fun", "ship_result_3")
                    : percentage > 10
                    ? StringResourceHandler.GetTextStatic("Fun", "ship_result_2")
                    : percentage > 0
                    ? StringResourceHandler.GetTextStatic("Fun", "ship_result_1")
                    : StringResourceHandler.GetTextStatic("Fun", "ship_result_0");
                if (percentage == 69) // Lenny Face
                    commenttext = StringResourceHandler.GetTextStatic("Fun", "ship_result_9");
                tstate.Dispose();
                await msg.DeleteAsync().ConfigureAwait(false);

                string progressbar = MatchmakingLogic.GenerateProgressBar(percentage, 100, 10);

                var resultembed = new EmbedBuilder()
                    .WithColor(Color.Magenta)
                    .AddField(StringResourceHandler.GetTextStatic("Fun", "ship_compatibility"), $"**{percentage}%** `{progressbar}` {commenttext}")
                    .Build();
                await ReplyAsync($"{StringResourceHandler.GetTextStatic("Fun", "ship_title")}\n{args[0]} :x: {args[1]}", false, resultembed);
            }
            else
            {
                
            }
        }
    }
}
