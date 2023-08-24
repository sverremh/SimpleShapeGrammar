using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Core;
using JMetalCSharp.Utils;

namespace ShapeGrammar.Classes
{
    class SH_Mutation : Mutation
    {
        // -- parameters --
        private static readonly List<Type> VALID_TYPES = new List<Type>() {typeof(SH_SolutionType) };

        private double? mutationProbability = null;

        // -- constructors --
        public SH_Mutation(Dictionary<string, object> parameters) : base(parameters)
        {
            //currently, no parameters are used
            
        }
        // -- methods --

        private void DoMutation(double probability, Solution solution)
        {
            // to do: implement this. 
        }


        public override object Execute(object obj)
        {
            Solution solution = (Solution)obj; // cast the object into a SH_Solution
            mutationProbability = 0.0; // to do: remove when implementing a mutation;

            // control that the type is valid for this mutation type
            if (!VALID_TYPES.Contains(solution.Type.GetType()))
            {
                // to do: log error message
                throw new Exception("The input Solution is not of the correct solutionType");
            }

            DoMutation(mutationProbability.Value, solution);
            return solution;
        }
    }
}
