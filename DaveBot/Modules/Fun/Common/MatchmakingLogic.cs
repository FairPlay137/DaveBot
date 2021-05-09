using System;
using System.Linq;

namespace DaveBot.Modules.Fun.Common
{
    //This code was mostly ripped from DaveBot Legacy
    public class MatchmakingLogic
    {
        private const int SUPER_MAGICAL_VALUE_WHICH_ACTUALLY_IMPACTS_RESULTS = 11;

        static int RandomNumberFromSeed(int seed, int range)
        {
            var LocalRNG = new Random(seed);
            return LocalRNG.Next(range);
        }

        public static string GenerateProgressBar(int value, int maxval, int chars)
        {
            var x = maxval / chars;
            var y = value / x;
            var z = "";
            while (y > 0)
            {
                z += "█";
                y--;
            }
            if ((value % x) >= (x / 2)) z += "█";
            while (z.Length < chars)
            {
                z += "▐";
            }
            return z;
        }

        public static int CalculateMatchmakingPercentage(string one, string two)
        {
            var sumofasciivalues1 = one.Aggregate(0, (current, c) => current + (byte) c);
            var sumofasciivalues2 = two.Aggregate(0, (current, c) => current + (byte) c);
            var finalsum = sumofasciivalues1 + sumofasciivalues2; //add checksums together

            var percentage = RandomNumberFromSeed(finalsum * SUPER_MAGICAL_VALUE_WHICH_ACTUALLY_IMPACTS_RESULTS, 101);

            return percentage;
        }
    }
}
