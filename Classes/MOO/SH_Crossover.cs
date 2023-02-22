using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JMetalCSharp.Core;
using JMetalCSharp.Operators.Crossover;
using JMetalCSharp.Utils;

using ShapeGrammar.Classes.Rules;

namespace ShapeGrammar.Classes
{
    class SH_Crossover : Crossover
    {
        // -- properties --

        private static readonly List<Type> VALID_TYPES = new List<Type>() { typeof(SH_SolutionType) };

        // -- constructors -- 

        public SH_Crossover(Dictionary<string, object> parameters) : base(parameters)
        {
            
        }

        // -- methods -- 

        private SH_Solution[] DoCrossover(SH_Solution parent1, SH_Solution parent2)
        {
            SH_NSGAIIProblem problem = (SH_NSGAIIProblem)parent1.sh_problem;
            
            SH_Solution[] offspring = new SH_Solution[2];
           
            // get the list of SH_Rule to use for identification of splicePoints
            List<SH_Rule> p1Genome = ((SH_Variable)parent1.SH_Variable[0]).RuleList;
            List<SH_Rule> p2Genome = ((SH_Variable)parent2.SH_Variable[0]).RuleList;

            // empty list for possible splice points 
            List<int[]> splicePoints = new List<int[]>();

            // identify all possible splice pairs. Not counting the last two rules in the genome as these are necessary to keep in place
            for (int i = 0; i < p1Genome.Count - 2; i++)
            {
                State p1CurrentState = p1Genome[i].RuleState; // the current state parent1
                for (int j = 0; j < p2Genome.Count - 2; j++)
                {
                    State p2CurrentState = p2Genome[j].RuleState; // the current state of parent2

                    if (p1CurrentState == p2CurrentState)
                    {
                        // if the states are compatible the rules can be joined here
                        splicePoints.Add(new int[] { i, j });
                    }
                }
            }

            // select a random splice pair
            int spliceInd = JMetalRandom.Next(0, splicePoints.Count-1); // compared to Random.Next(), the JMetalRandom.Next() method gives the maximum value equal to the max input, not smaller than. 
            int[] splicePair = splicePoints[spliceInd]; // select the random splice points

            // split the list
            List<SH_Rule> p1_1 = p1Genome.GetRange(0, splicePair[0] + 1);
            List<SH_Rule> p1_2 = p1Genome.GetRange(splicePair[0]+1, p1Genome.Count - p1_1.Count);

            List<SH_Rule> p2_1 = p2Genome.GetRange(0, splicePair[1] + 1);
            List<SH_Rule> p2_2 = p2Genome.GetRange(splicePair[1] +1, p2Genome.Count - p2_1.Count);

            List<SH_Rule> o1Genome = p1_1.Concat(p2_2).ToList(); // genome for the first offspring
            List<SH_Rule> o2Genome = p2_1.Concat(p1_2).ToList(); // genome for the second offspring

            // create the offspring
            SH_Variable[] o1Var = new SH_Variable[1] {new SH_Variable(o1Genome) }; // create a variable of the first offspring
            SH_Variable[] o2Var = new SH_Variable[1] { new SH_Variable(o2Genome) }; // create a variable of the second offspring

            offspring[0] = new SH_Solution(problem, o1Var);
            offspring[1] = new SH_Solution(problem, o2Var);

            SG_Shape ss01 =
                Util.ApplyRulesToSimpleShape(o1Var[0].RuleList, parent1.sh_problem.MyComponent.SimpleShape);
            offspring[0].SH_Variable[0].SimpleShape = ss01;
            SG_Shape ss02 =
                Util.ApplyRulesToSimpleShape(o2Var[0].RuleList, parent2.sh_problem.MyComponent.SimpleShape.DeepCopy());
            offspring[1].SH_Variable[0].SimpleShape = ss02;

            //offspring[1] = new SH_Solution(o2Var); // the constructor should take the list of SH_Rule as input

            return offspring;
        }

        public override object Execute(object obj)
        {
            SH_Solution[] parents = (SH_Solution[])obj; // cast the input to the correct type

            if(parents.Length != 2)
            {
                // to do: Create log message
                throw new Exception("Exception in " + this.GetType().FullName + ".Excecute");
            }

            // control that the input is a valid type
            if (!(VALID_TYPES.Contains(parents[0].Type.GetType()) && VALID_TYPES.Contains(parents[1].Type.GetType())))
            {
                // to do: create log message
                throw new Exception("Exception in " + this.GetType().FullName + ".Excecute():" + " The input SolutionType is not correct.");
            }
            
            SH_Solution[] offspring;
            offspring = DoCrossover(parents[0], parents[1]);

            return offspring;
        }
    }
}
