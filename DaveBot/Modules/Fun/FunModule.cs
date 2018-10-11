using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DaveBot.Common;
using DaveBot.Services;

namespace DaveBot.Modules
{
    public class Fun : DaveBotModuleBase<SocketCommandContext>
    {
        string[] DefaultEightBallResponses = { "Yes.", "No.", "Ask again later.", "Maybe???", "It is certain", "Very doubtful",
                                        "NO - It'll cause a time paradox.", "Chances are as high as the sky.", "lolnope",
                                        "Obviously.", "Don't count on it.", "I'd say so.", "Received interference; try again."};

        private readonly IBotConfiguration _config;

        public Fun(IBotConfiguration config)
        {
            _config = config;
        }

        [Command("8ball")]
        [Summary("Ask the 8ball a question!")]
        public async Task EightBall([Remainder] [Summary("The question")] string question)
        {
            Random random = new Random();
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
            Random random = new Random();
            string[] optionsIndiv = options.Split(';');
            string choice = optionsIndiv[random.Next(optionsIndiv.Length)];
            await ReplyAsync("", false, new EmbedBuilder()
                .AddField(":thinking:", choice)
                .Build());
        }
    }
}
