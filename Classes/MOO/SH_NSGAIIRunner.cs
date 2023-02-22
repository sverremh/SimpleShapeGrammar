using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JMetalCSharp.Core;
using JMetalCSharp.Operators.Crossover;
using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Operators.Selection;
using JMetalCSharp.QualityIndicator;
using ShapeGrammar.Components.MOOComponents;

namespace ShapeGrammar.Classes
{
    
    class SH_NSGAIIRunner
    {
        // - properties --
        // FirstGrammarMOO MyComponent;

        // -- constructor --
        public SH_NSGAIIRunner(string[] args, SH_NSGAIIProblem _problem, string Path, FirstGrammarMOO component)
        {
            SH_NSGAIIProblem problem = _problem;

            //algorithm
            SH_NSGAII algorithm;
            Operator crossover;
            Operator selection;
            Operator mutation;
            //MyComponent = component;

            Dictionary<string, object> parameters; // operator parameters

            QualityIndicator indicators;
            indicators = null;

            algorithm = new SH_NSGAII(problem);

            // set algorithm parameters
            algorithm.SetInputParameter("populationSize", problem.MyComponent.populationSize);
            algorithm.SetInputParameter("maxEvaluations", problem.MyComponent.maxEvals);
            // to do: log the messages

            // set mutation and crossover for SH_Grammar 
            parameters = new Dictionary<string, object>();
            parameters.Add("probability", 0.9);
            parameters.Add("distributionIndex", 20.0);
            crossover = SH_CrossoverFactory.GetCrossoverOperator("SH_Crossover", parameters); // The parameters might not be necessary.
            //crossover = CrossoverFactory.GetCrossoverOperator(); // Create a crossover for SH_Grammar


            parameters = new Dictionary<string, object>();
            parameters.Add("probability", 0.9);
            parameters.Add("distributionIndex", 20.0);
            mutation = SH_MutationFactory.GetMutationOperator("SH_Mutation", parameters);
            // mutation = MutationFactory.GetMutationOperator(); // Create a mutation for SH_Grammar // if everything works this can be deleted. 

            parameters = null;
            selection = SelectionFactory.GetSelectionOperator("BinaryTournament2", parameters); // Maybe I can use this in the beginning. 

            // add the three operators to the algorithm
            algorithm.AddOperator("crossover", crossover); algorithm.AddOperator("mutation", mutation); algorithm.AddOperator("selection", selection);

            // add the indicators 
            algorithm.SetInputParameter("indicators", indicators);

            // excecute the algorithm
            long initTime = Environment.TickCount;
            SolutionSet population = algorithm.Execute();
            long estimatedTime = Environment.TickCount - initTime; 

            // to do: log info about the solver

                
        }
        // -- methods --
    }
}
