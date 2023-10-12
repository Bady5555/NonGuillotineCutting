using System.Security.Cryptography;

namespace NonGuillotineCutting.Utils
{
    public static class ListExtension
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> vals)
        {
            int n = vals.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);

                T value = vals[k];

                vals[k] = vals[n];
                vals[n] = value;
            }
        }
    }
}
