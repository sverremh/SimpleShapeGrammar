using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel.Special;

using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils.Wrapper;

namespace SimpleShapeGrammar.Classes
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
        List<double> objectives = new List<double>();

        public List<(List<SH_Rule> genome, List<double> fitness)> allSolutions = new List<(List<SH_Rule> genome, List<double> fitness)>(); // a list of tuples; 

        // -- constructors -- 
        public SH_NSGAIIProblem(string solutionType, FirstGrammarMOO comp, int solutionsCounter)
        {
            MyComponent = comp;
            //variableSliders = comp.ReadSlidersList();
            //NumberOfVariables = variableSliders.Count;
            NumberOfObjectives = comp.objectives.Count;
            NumberOfConstraints = 0;
            availableRules = MyComponent.GrammarRules;
            weights = MyComponent.GrammarWeights;
            ProblemName = "MultiObjective";

            // Should add some log messages here at some point. 

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
            if (solutionType == "SH_SolutionType")
            {
                SolutionType = new SH_SolutionType(this); 
            }
            else
            {
                Console.WriteLine("Error: solution type " + solutionType + " is invalid; please try another solution");
                //comp.LogAddMessage("Error: solution type " + solutionType + " is invalid; please try another solution");
                return;
            }
            //comp.LogAddMessage("Solution type: " + solutionType);
        }
        // -- methods --
        public override void Evaluate(Solution solution)
        {
            throw new NotImplementedException();
        }
    }
}
