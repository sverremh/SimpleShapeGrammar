using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Utils;

namespace SimpleShapeGrammar.Classes.MOO
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

        private void DoMutation(double probability, SH_Solution solution)
        {
            // to do: implement this. 
        }


        public override object Execute(object obj)
        {
            SH_Solution solution = (SH_Solution)obj; // cast the object into a SH_Solution

            // control that the type is valid for this mutation type
            throw new NotImplementedException();
        }
    }
}
