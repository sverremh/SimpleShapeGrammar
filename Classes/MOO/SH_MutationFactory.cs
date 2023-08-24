using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JMetalCSharp.Operators.Mutation;

namespace ShapeGrammar.Classes
{
    public static class SH_MutationFactory
    {
        public static Mutation GetMutationOperator(string name, Dictionary<string, object> parameters)
        {
            if (name.Equals("SH_Mutation", StringComparison.InvariantCultureIgnoreCase))
            {
                return new SH_Mutation(parameters);
            }
            else
            {
                // to do: log message
                throw new Exception("The input Mutation is not a valid one.");
            }
                
        }
    }
}
