namespace DaveBot
{
    class Program
    {
        static void Main(string[] args)
            => new DaveBot().StartAndBlockAsync(args).GetAwaiter().GetResult();
    }
}
