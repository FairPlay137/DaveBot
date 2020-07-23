using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DaveBot.Services;
using DaveBot.Modules.Fun.Common;
using DaveBot.Modules.Fun.Services;
using DaveBot.Common;

namespace DaveBot.Modules
{
    [Name("Fun")]
    public partial class FunModule : DaveBotTopModuleBase
    {
        string[] DefaultEightBallResponses = { "It is certain.", "Without a doubt.", "You may rely on it.", "Most likely.", "Outlook good.",
                                        "Yes.", "Don't count on it.", "My reply is no.", "My sources say no.", "Outlook not so good.",
                                        "Very doubtful.", "Reply hazy; try again.", "Ask again later.", "Better not tell you now.",
                                        "Cannot predict now.", "Concentrate and ask again."};

        string[] DefaultReviveChatTopics = { "The last video game you played is the world you somehow end up in. How well do you think the experience would go?",
                                        ""};

        private readonly IBotConfiguration _config;

        public FunModule(IBotConfiguration config)
        {
            _config = config;
        }

        [Command("8ball")]
        [Summary("Ask the 8ball a question!")]
        public async Task EightBall([Remainder] [Summary("The question")] string question)
        {
            await Context.Message.AddReactionAsync(new Emoji("🎱"));
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
            await Context.Message.AddReactionAsync(new Emoji("🤔"));
            DaveRNG random = new DaveRNG();
            string[] optionsIndiv = options.Split(';');
            string choice = optionsIndiv[random.Next(optionsIndiv.Length)];
            await ReplyAsync("", false, new EmbedBuilder()
                .AddField(":thinking:", choice)
                .WithColor(Color.Blue)
                .Build());
        }
        [Command("ship")]
        [Summary("Checks the compatibility level of two people.")]
        public async Task Ship([Remainder] [Summary("Two targets, separated by `;`")] string targets)
        {
            string[] args = targets.Split(';');
            if (args.Length == 2)
            {
                await Context.Message.AddReactionAsync(new Emoji("❤"));
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
                await Context.Message.AddReactionAsync(new Emoji("⛔"));
                await ReplyAsync($":no_entry: `{StringResourceHandler.GetTextStatic("Fun", "ship_incorrectNumOfArgs")}`").ConfigureAwait(false);
            }
        }

        /*[Command("reviveChat")]
        [Summary("Generates a random topic to attempt to bring life back to a dead chat.")]*/
    }
}
