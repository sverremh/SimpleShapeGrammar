using System;
using System.Collections.Generic;
using System.Linq;

using JMetalCSharp.Operators;
using JMetalCSharp.Operators.Crossover;

namespace ShapeGrammar.Classes
{
    public static class SH_CrossoverFactory
    {
        public static Crossover GetCrossoverOperator(string name, Dictionary<string, object> parameters)
        {
            if (name.Equals("SH_Crossover", StringComparison.InvariantCultureIgnoreCase))
                return new SH_Crossover(parameters);
            else
            {
                // to do: add log message
                throw new Exception("Exception in SH_CrossoverFactory.GetCrossoverOperator(): Operator " + name + " not defined as a Crossover type");
            }
        }
    }
}
