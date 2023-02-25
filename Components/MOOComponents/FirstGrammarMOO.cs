using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
using ShapeGrammar.Classes;
using ShapeGrammar.Classes.Rules;

namespace ShapeGrammar.Components.MOOComponents
{
    public class FirstGrammarMOO : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the FirstGrammarMOO class.
        /// </summary>
        public FirstGrammarMOO()
          : base("FirstGrammarMOO", "Nickname",
              "Description",
              "SimpleGrammar", "MOO")
        {
            ObjectiveValues = new List<List<double>>();
            ObjectiveVariables = new List<List<SG_Rule>>();
            MyRand = new Random();
            comparer = new ObjectiveComparer();
            GrammarRules = new List<object>();
            GrammarWeights = new List<double>(); 
            
        }

        // create new component attribute
        public override void CreateAttributes()
        {
            base.m_attributes = new MOOComponentAttributes(this);
        }


        // -- properties --
        public SG_Shape SimpleShape { get; private set;  }
        public bool mooDone = false;
        public List<List<double>> ObjectiveValues;
        public List<List<SG_Rule>> ObjectiveVariables;
        public List<object> GrammarRules;
        public List<double> GrammarWeights;
        public List<string> GrammarObjectives;
        public List<GH_NumberSlider> slidersList = new List<GH_NumberSlider>();
        public Random MyRand;
        public int Seed;
        public string log = null;
        private ObjectiveComparer comparer;
        public int solutionsCounter = 0;
        public int populationSize = 0, generations = 0, maxEvals = 0;
        

        // create data tree of solutions as global variables
        public DataTree<double> outObjectiveTree;
        public DataTree<SG_Rule> outRules;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("SimpleShape", "ss", "The basis shape to be used in the optimisation", GH_ParamAccess.item); // 0
            pManager.AddGenericParameter("Rules", "rules", "List of rules applicable for the grammar", GH_ParamAccess.list); // 1
            pManager.AddNumberParameter("Weights", "weights", "List of probabilities for the random selection of each rule", GH_ParamAccess.list); // 2
            pManager.AddIntegerParameter("Population", "pop", "Number of individuals in a generation", GH_ParamAccess.item, 10); // 3
            pManager.AddIntegerParameter("Generations", "gen", "Number of generations", GH_ParamAccess.item, 5); // 4
            pManager.AddIntegerParameter("Seed", "sd", "Seed for Random", GH_ParamAccess.item, 0); // 5
            pManager.AddBooleanParameter("Run optimization", "run", "Set input to true for the component to run", GH_ParamAccess.item); //6
            pManager.AddBooleanParameter("Reset Solver", "reset", "Press the button to clear the results and reset the solver", GH_ParamAccess.item); // 7

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Grammar derivation", "lst", "The rules applicable for the grammar derivation", GH_ParamAccess.tree); // 0
            pManager.AddGenericParameter("Objectives", "obj", "Objective values from each generation", GH_ParamAccess.tree); // 1
            pManager.AddGenericParameter("Test output", "test", "", GH_ParamAccess.tree); // 2
            pManager.AddGenericParameter("Test Karamba", "", "", GH_ParamAccess.tree); //3
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            SG_Shape ss = new SG_Shape(); // 0
            List<SG_Rule> rulesList = new List<SG_Rule>(); // 1
            List<double> weights = new List<double>();
            bool run = false;
            bool reset = false;
            // --- input ---
            if (!DA.GetData(0, ref ss)) return; // 0
            if (!DA.GetDataList(1, rulesList)) return;  // 1
            DA.GetDataList(2, weights); // 2
            DA.GetData(3, ref populationSize); // 3
            DA.GetData(4, ref generations); // 4
            DA.GetData(5, ref Seed); // 5
            if (!DA.GetData(6, ref run)) return; // 6
            DA.GetData(7, ref reset); // 7
            

            maxEvals = populationSize * generations; // total number of evaluations

            //Create a deep copy of the simple Shape before performing rule operations
            SG_Shape copyShape = Util.DeepCopy(ss);
            SimpleShape = copyShape; // assign the SH_SimpleShape instance to the component's property

            // Control the input
            if (populationSize % 2 == 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The population size must be an even number.");
                return;
            }

            // make the run boolean toggle a global parameter
            var runBool = (GH_BooleanToggle)this.Params.Input[6].Sources[0];
            if (runBool == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The \"Run Optimisation\" input have to be a Boolean Toggle");
                return;
            }

            // access the Active GH Document
            var doc = OnPingDocument();
            if (doc == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Something is wrong when calling the document.");
                return;
            }

            if (reset)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Solution is reset.");
                //ObjectiveVariables = new List<List<SH_Rule>>(); // reset list of variables
                //ObjectiveValues = new List<List<double>>(); // reses list of values
                runBool.Value = false;
                mooDone = false;
                GrammarWeights = new List<double>();
                outRules = new DataTree<SG_Rule>();
                outObjectiveTree = new DataTree<double>();
                runBool.ExpireSolution(true);

            }

            // Is this clause necessary?
            if (!run && !mooDone) // Make sure to include the "mooDone" here to avoid a complete rerun if the user refresh the solution
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Set boolean to \"True\" to run the component.");
                outRules = new DataTree<SG_Rule>();
                outObjectiveTree = new DataTree<double>();
            }

            if (Seed != 0) { MyRand = new Random(Seed); } // reset Random to give same result each time

            // --- solve ---
            DataTree<SG_Rule> genomeTree = new DataTree<SG_Rule>();
            DataTree<double> objValTree = new DataTree<double>(); 
            if (run & !mooDone)
            {
                // instantiate the solver

                // Select rules
                //GrammarRules = new List<object>() { "SH_Rule01", "SH_Rule02", "SH_Rule03", "SH_RuleA" }; // select the available rules for the interpreter
                //GrammarRules = RuleNamesList(rulesList);
                GrammarRules = new List<object>(rulesList);
                // Assign weights to the rules. 
                // Make weights based on the list length. Should be optional input after a while. 
                GrammarWeights = CreateWeights(GrammarRules, weights);
                //GrammarWeights = new List<double>() { 0.1, 0.6, 0.15, 0.15 }; // the list of probabilities for each rule to be selected
                
                // Select optimisation objectives
                GrammarObjectives = new List<string>() { "max displacement", "total mass" };

                SH_NSGAIIProblem problem = new SH_NSGAIIProblem("SH_Solution", this, solutionsCounter);
                SH_NSGAIIRunner runner = new SH_NSGAIIRunner(null, problem, null, this);
                var allSolutions = problem.allSolutions;
                var paretoSolutions = allSolutions.GetRange(allSolutions.Count - 1 - populationSize, populationSize);

                
                
                // set the "Run optimisation" boolean input to False for user safety. 
                //var runBool = (GH_BooleanToggle) this.Params.Input[5].Sources[0];
                if (runBool != null) { runBool.Value = false;}

                mooDone = true;

                var callbackDelegate = new GH_Document.GH_ScheduleDelegate(Callback);
                doc.ScheduleSolution(6, callbackDelegate);
                GetDataTreesFromResults(allSolutions, out outRules, out outObjectiveTree);
            }
            

            DA.SetDataTree(2, outRules);
            DA.SetDataTree(3, outObjectiveTree);
            

        }

        #region Methods

        /// <methods>
        /// Methods used in the main solve instance
        /// </methods>
        private List<object> RuleNamesList(List<SG_Rule> rules)
        {

            List<object> names = new List<object>();

            foreach (SG_Rule rule in rules)
            {
                names.Add(rule.Name);
            }

            return names;
        }

        private List<double> CreateWeights(List<object> rules, List<double> weightsList)
        {
            int count = rules.Count;

            if (weightsList.Count != count)
            {
                double step = 1.0 / (double)count;

                List<double> weights = new List<double>();
                for (int i = 0; i < count; i++)
                {
                    weights.Add(step);
                }

                return weights;
            }
            else
            {
                return weightsList;
            }
            
        }

        private void Callback(GH_Document doc)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Something has changed");
            var runBool = this.Params.Input[6].Sources[0] as GH_BooleanToggle;
            runBool.ExpireSolution(true);
            //this.Params.Input[2].RemoveAllSources();
        }

        public List<GH_NumberSlider> ReadSlidersList()
        {
            slidersList.Clear(); // clear the values from the list
            foreach (IGH_Param param in Params.Input[0].Sources) // iterate through the input sources of the first input, i.e. variables
            {
                GH_NumberSlider slider = param as GH_NumberSlider;
                slidersList.Add(slider);
            }
            return slidersList;
        }

        private void GetDataTreesFromResults(List<(List<SG_Rule> genome, List<double> fitness)> allSolutions, out DataTree<SG_Rule> genomes, out DataTree<double> objFitnessValues)
        {
            DataTree<SG_Rule> tree1 = new DataTree<SG_Rule>(); // tree for list of rules
            DataTree<double> tree2 = new DataTree<double>(); // tree for list of fitness values

            // iterate through the list of tuples
            for (int i = 0; i < allSolutions.Count; i++)
            {
                GH_Path path = new GH_Path(i);
                tree1.AddRange(allSolutions[i].genome, path);
                tree2.AddRange(allSolutions[i].fitness, path);
            }

            genomes = tree1;
            objFitnessValues = tree2;
        }
        

        #endregion

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("69ba0c70-2c65-4e31-8a83-3f7b4e10d828"); }
        }
    }
}