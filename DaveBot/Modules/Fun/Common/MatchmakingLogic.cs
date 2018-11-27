﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveBot.Modules.Fun.Common
{
    //This code was ripped from DaveBot Legacy
    public class MatchmakingLogic
    {
        static int RandomNumberFromSeed(int seed, int range)
        {
            Random LocalRNG = new Random(seed);
            return LocalRNG.Next(range);
        }

        public static string GenerateProgressBar(int value, int maxval, int chars)
        {
            int x = maxval / chars;
            int y = value / x;
            string z = "";
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
            int sumofasciivalues1 = 0;
            int sumofasciivalues2 = 0;

            foreach (char c in one)
                sumofasciivalues1 += (byte)c; //determine checksums
            foreach (char c in two)
                sumofasciivalues2 += (byte)c;
            int finalsum = sumofasciivalues1 + sumofasciivalues2; //add checksums together

            int percentage = RandomNumberFromSeed(finalsum * 11, 101);

            return percentage;
        }
    }
}