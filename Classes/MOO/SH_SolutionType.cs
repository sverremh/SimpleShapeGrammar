using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JMetalCSharp.Core;

namespace ShapeGrammar.Classes
{
    class SH_SolutionType : SolutionType
    {
        // -- properties -- 

        // -- constructors -- 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="problem"></param>
        public SH_SolutionType(SH_NSGAIIProblem problem) : base(problem)
        {
            Problem = problem; 
        }
        // -- methods -- 

        /// <summary>
        /// Creates the variables of the solution
        /// </summary>
        /// <returns></returns>
        public override Variable[] CreateVariables()
        {
            Variable[] variables = new Variable[1];
            variables[0] = new SH_Variable( (SH_NSGAIIProblem) Problem);
            return variables; 
        }
    }
}
