using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JMetalCSharp.Core;

namespace ShapeGrammar.Classes
{
    public class SH_Solution : Solution 
    {
        // test if this class is unnecessary

        // -- properties --
        public SH_NSGAIIProblem sh_problem { get; private set; }

        public SH_Variable[] SH_Variable { get; set; }

        private int numberOfObjectives;
        
        new private int NumberOfObjectives
        {
            get
            {
                if (this.Objective == null)
                {
                    return 0;
                }
                else
                {
                    return numberOfObjectives;
                };
            }
        }
        /*
        double[] o;
        new public double[] Objective
        {
            get
            {
                return o;
            }
            set
            {
                o = value;
            }
        }*/

        // -- constructors --

        public SH_Solution(SH_Variable[] variables)
        {
            Variable = variables;
        }
        
        public SH_Solution(SH_NSGAIIProblem _problem)
        {
            sh_problem = _problem;
            Type = _problem.SolutionType;
            numberOfObjectives = _problem.NumberOfObjectives;
            Objective = new double[numberOfObjectives];
            
            Fitness = 0.0;
            KDistance = 0.0;
            CrowdingDistance = 0.0;
            DistanceToSolutionSet = double.PositiveInfinity;
            SH_Variable = new SH_Variable[] { new SH_Variable(_problem) };
            // Need to create the variabel

        }
        public SH_Solution(Problem _problem, SH_Variable[] variables)
        {
            sh_problem = (SH_NSGAIIProblem)_problem;
            Type = _problem.SolutionType;
            numberOfObjectives = _problem.NumberOfObjectives;
            Objective = new double[numberOfObjectives];

            Fitness = 0.0;
            KDistance = 0.0;
            CrowdingDistance = 0.0;
            DistanceToSolutionSet = double.PositiveInfinity;
            SH_Variable = variables;
        }
        // -- methods
        
    }
}
