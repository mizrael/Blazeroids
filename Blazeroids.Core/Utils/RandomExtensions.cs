using System;

namespace Blazeroids.Core.Utils
{
    public static class RandomExtensions
    {
        public static double NextDouble(
            this Random random,
            double minValue, double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }

        /// returns a random number between two intervals
        public static double NextDouble(
            this Random random,
            double minValue1, double maxValue1,
            double minValue2, double maxValue2)
        {
            return random.NextBool() ?
                random.NextDouble(minValue1, maxValue1) :
                random.NextDouble(minValue2, maxValue2);
        }

        public static bool NextBool(this Random random) => (1 == (random.Next() & 1));
    }
}