using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DaveBot.Common;
using DaveBot.Services;

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
                .Build());
        }
    }
}
