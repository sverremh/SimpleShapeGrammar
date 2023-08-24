using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.IO;
using JMetalCSharp.Core;
using JMetalCSharp.QualityIndicator;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Comparators;
using ShapeGrammar.Components.MOOComponents;

namespace ShapeGrammar.Classes
{
    class SH_NSGAII: Algorithm
    {
        // -- properties --

        private SH_NSGAIIProblem SH_Problem;
        // -- constructors --
        public SH_NSGAII(SH_NSGAIIProblem problem) : base(problem)
        {
            SH_Problem = problem; 
        }

        // -- methods --
        public override SolutionSet Execute()
        {
            int populationSize = -1;
            int maxEvaluations = -1;
            int evaluations;

            QualityIndicator indicators = null;
            int requiredEvaluations;

            SolutionSet population;
            SolutionSet offspringPopulation;
            SolutionSet union;

            Operator mutationOperator;
            Operator crossoverOperator;
            Operator selectionOperator;

            Distance distance = new Distance();

            // read the parameters
            Utils.GetIntValueFromParameter(InputParameters, "maxEvaluations", ref maxEvaluations);
            Utils.GetIntValueFromParameter(InputParameters, "populationSize", ref populationSize);
            Utils.GetIndicatorsFromParameters(InputParameters, "indicators", ref indicators);

            // initialise the variables
            population = new SolutionSet(populationSize);
            evaluations = 0;

            requiredEvaluations = 0;

            // read the operators
            mutationOperator = Operators["mutation"];
            crossoverOperator = Operators["crossover"];
            selectionOperator = Operators["selection"];

            // create the first population
            JMetalRandom.SetRandom(SH_Problem.MyComponent.MyRand);


            // create the initial solution set
            //Solution newSolution;
            for (int i = 0; i < populationSize; i++)
            {
                //newSolution = new SH_Solution(Problem);
                var newSolution = new SH_Solution((SH_NSGAIIProblem)Problem); // there is some dependencies here which writes over previous solutions
                SH_Problem.SH_Evaluate(newSolution);
                SH_Problem.EvaluateConstraints(newSolution);
                evaluations++;
                population.Add(newSolution); // try to add a copy
                
            }

            List<double> objectives1 = new List<double>();
            List<double> objectives2 = new List<double>();
            foreach (SH_Solution solution in population.SolutionsList)
            {
                objectives1.Add(solution.Objective[0]);
                objectives2.Add(solution.Objective[1]);
            }


            // iterate through the generations
            while (evaluations < maxEvaluations)
            {
                // create the offspring solutionSet
                offspringPopulation = new SolutionSet(populationSize);
                SH_Solution[] parents = new SH_Solution[2];
                for (int i = 0; i < populationSize/2; i++)
                {
                    if (evaluations < maxEvaluations)
                    {
                        // obtain parents
                        parents[0] = (SH_Solution)selectionOperator.Execute(population);
                        parents[1] = (SH_Solution)selectionOperator.Execute(population);

                        // do crossover
                        SH_Solution[] offspring = (SH_Solution[])crossoverOperator.Execute(parents);

                        mutationOperator.Execute(offspring[0]);
                        mutationOperator.Execute(offspring[1]);

                        //evaluate the offspring
                        SH_Problem.SH_Evaluate(offspring[0]);
                        SH_Problem.EvaluateConstraints(offspring[0]);
                        SH_Problem.SH_Evaluate(offspring[1]);
                        SH_Problem.EvaluateConstraints(offspring[1]);

                        // add offspring to to new offspring population
                        offspringPopulation.Add(offspring[0]);
                        offspringPopulation.Add(offspring[1]);

                        evaluations += 2;
                    }
                    
                }
                // create the solutionSet union of parents and offspring
                union = ((SolutionSet)population).Union(offspringPopulation);

                // ranking the union
                Ranking ranking = new Ranking(union);

                int remain = populationSize;
                int index = 0;
                SolutionSet front = null;
                population.Clear(); // clear the current population

                // obtain the next front
                front = ranking.GetSubfront(index);

                while ((remain > 0) && (remain >= front.Size()))
                {
                    // assign crowding distance to individuals
                    distance.CrowdingDistanceAssignment(front, Problem.NumberOfObjectives);
                    // add the individuals to this front
                    for (int k = 0; k < front.Size(); k++)
                    {
                        population.Add(front.Get(k));
                    }

                    // decrement remain
                    remain -= front.Size();

                    // obtain next front
                    index++;
                    if (remain>0)
                    {
                        front = ranking.GetSubfront(index);
                    }
                }

                // if "remain" is less than the front(index).Size(), insert only the best one. 
                if (remain > 0)
                {
                    distance.CrowdingDistanceAssignment(front, Problem.NumberOfObjectives);
                    front.Sort(new CrowdingComparator());
                    for (int k = 0; k < remain; k++)
                    {
                        population.Add(front.Get(k));
                    }
                    remain = 0;
                }

                // This piece of code shows how to use the indicator object into the code
                // of NSGA-II. In particular, it finds the number of evaluations required
                // by the algorithm to obtain a Pareto front with a hypervolume higher
                // than the hypervolume of the true Pareto front.
                if ((indicators != null) && (requiredEvaluations != 0))
                {
                    double HV = indicators.GetHypervolume(population);
                    if (HV >= (0.98 * indicators.TrueParetoFrontHypervolume))
                    {
                        requiredEvaluations = evaluations;
                    }
                }
            }

            // return as output parameter the required evaluations
            SetOutputParameter("evaluations", requiredEvaluations);

            // return th first non-dominated front
            Ranking rank = new Ranking(population);
            Result = rank.GetSubfront(0);

            return Result;

            
        }
    }
}
