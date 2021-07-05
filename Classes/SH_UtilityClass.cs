using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SimpleShapeGrammar.Classes
{

    // --- variables ---

    // --- input ---

    // --- solve ---

    // --- output ---


    // --- properties ---

    // --- constructors ---

    // --- methods ---

    public static class SH_UtilityClass
    {
        public static T DeepCopy<T>(T target)
        {
            T result;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream m = new MemoryStream();

            try
            {
                bf.Serialize(m, target);
                m.Position = 0;
                result = (T)bf.Deserialize(m);
            }
            finally
            {
                m.Close();
            }



            return result;
        }

        public static void TakeRandomItem(List<object> fromList, List<double> weights, Random random, out object item)
        {
            // find the sum of weights
            double sum_of_weights = weights.Sum();
            object el = new object();
            int ind = 0;
            // initiate random
            //var random = new Random();
            double rnd = RandomExtensions.NextDouble(random, 0, sum_of_weights);
            //Console.WriteLine("Random number selected: {0}", rnd);
            for (int i = 0; i < weights.Count; i++)
            {
                if (rnd < weights[i])
                {
                    el = fromList[i];
                    ind = i;
                    break;
                }
                rnd -= weights[i];
            }
            item = el;
        }

        public static class RandomExtensions
        {

            public static double NextDouble(Random random, double min, double max)
            {
                return random.NextDouble() * (max - min) + min;
            }
        }
    }
}
