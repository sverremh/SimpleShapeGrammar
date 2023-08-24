using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel.Special;

using Karamba.Models;

using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils.Wrapper;
using ShapeGrammar.Classes.Rules;
using ShapeGrammar.Components.MOOComponents;

namespace ShapeGrammar.Classes
{
    public class SH_NSGAIIProblem : Problem
    {
        // -- properties --
        public FirstGrammarMOO MyComponent = null;
        List<double> var1Value = new List<double>();
        List<double> var2Value = new List<double>();
        List<double> objectiveValue = new List<double>();
        //List<GH_NumberSlider> variableSliders = new List<GH_NumberSlider>(); // maybe this needs to be changed into a list of the available rules

        public List<object> availableRules; // the rules which can be selected in the generation of a genome.
        public List<double> weights; // propability of selecting each rule
        //public List<string> objectives; // string list of possible objectives
        //List<double> objectives = new List<double>();

        public List<(List<SG_Rule> genome, List<double> fitness)> allSolutions = new List<(List<SG_Rule> genome, List<double> fitness)>(); // a list of tuples containing all the solutions 

        // -- constructors -- 
        /// <summary>
        /// Instantiate a SH_NSGAIIProblem instance
        /// </summary>
        /// <param name="solutionType"></param>
        /// <param name="comp"></param>
        /// <param name="solutionsCounter"></param>
        public SH_NSGAIIProblem(string solutionType, FirstGrammarMOO comp, int solutionsCounter)
        {
            MyComponent = comp;
            //variableSliders = comp.ReadSlidersList();
            NumberOfVariables = 1; // only the list of rules are acting as a variable for now. 
            NumberOfObjectives = MyComponent.GrammarObjectives.Count;
            NumberOfConstraints = 0;
            availableRules = MyComponent.GrammarRules;
            weights = MyComponent.GrammarWeights;
            ProblemName = "GrammarOptimisation";

            // to do: add log messages

            UpperLimit = new double[NumberOfVariables];
            LowerLimit = new double[NumberOfVariables];

            // get the upper and lower bounds of the sliders
            /*
            for (int i = 0; i < NumberOfVariables; i++)
            {
                GH_NumberSlider slider = variableSliders[i];
                UpperLimit[i] = (double)slider.Slider.Maximum;
                LowerLimit[i] = (double)slider.Slider.Minimum;

            }*/

            if (solutionType == "BinaryReal")
            {
                SolutionType = new BinaryRealSolutionType(this);
            }
            if (solutionType == "Real")
            {
                SolutionType = new RealSolutionType(this);
            }
            if (solutionType == "ArrayReal")
            {
                SolutionType = new ArrayRealSolutionType(this);
            }
            // Add the grammar spesific solution type here
            if (solutionType == "SH_Solution")
            {
                SolutionType = new SH_SolutionType(this); 
            }
            else
            {
                Console.WriteLine("Error: solution type " + solutionType + " is invalid; please try another solution");
                // to do: add log message
                //comp.LogAddMessage("Error: solution type " + solutionType + " is invalid; please try another solution");
                return;
            }
            //comp.LogAddMessage("Solution type: " + solutionType);
            // to do: add log message
        }
        // -- methods --
        public void SH_Evaluate(SH_Solution solution)
        {
            //SH_Solution shSolution = solution as SH_Solution;
            // the current solution to evaluate
            (List<SG_Rule> ruleList, List<double> objectiveValues) currentSolution; // the current solution to evaluate

            SH_XReal x = new SH_XReal(solution); // using the wrapper when working with the solution. I will probably have to modify the wrapper as well for it to be compatible with shape grammars

            List<SG_Rule> ruleList = x.GetRuleList(); // gets the list of rules used by this variable

            // modify the initial simple shape by the list of rules
            //SH_SimpleShape simpleShape = MyComponent.SimpleShape;
            //SH_SimpleShape ssCopy = SH_UtilityClass.DeepCopy(MyComponent.SimpleShape);
            //SH_SimpleShape grammarShape = SH_UtilityClass.ApplyRulesToSimpleShape(ruleList, ssCopy);

            // create a karamba model from the SH_SimpleShape
            //Model karambaModel = SH_UtilityClass.Karamba3DModelFromSimpleShape(grammarShape);
            Model karambaModel = UT.Karamba3DModelFromSimpleShape(x.GetSimpleShape());
            // analyse the karamba model and return the objective functions
            List<double> objectives = UT.AnalyseKarambaModel(MyComponent.GrammarObjectives, karambaModel);

            // assign the objective values to the solution
            for (int i = 0; i < MyComponent.GrammarObjectives.Count; i++)
            {
                solution.Objective[i] = objectives[i];
            }
            

            // add the evaluated solutions to all solutions
            
            currentSolution.ruleList = ruleList;
            currentSolution.objectiveValues = objectives; 
            allSolutions.Add(currentSolution);

            
        }

        public override void Evaluate(Solution solution)
        {
            throw new NotImplementedException();
        }
    }
}
