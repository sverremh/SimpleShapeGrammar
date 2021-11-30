using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Data;
using Grasshopper;
using Karamba.Models;



namespace SimpleShapeGrammar
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
            ObjectiveVariables = new List<List<SH_Rule>>();
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
        public SH_SimpleShape SimpleShape { get; private set;  }
        public bool mooDone = false;
        public List<List<double>> ObjectiveValues;
        public List<List<SH_Rule>> ObjectiveVariables;
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
        



        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("SimpleShape", "ss", "The basis shape to be used in the optimisation", GH_ParamAccess.item); // 0
            pManager.AddGenericParameter("Rules", "rules", "List of rules applicable for the grammar", GH_ParamAccess.list); // 1
            pManager.AddIntegerParameter("Population", "pop", "Number of individuals in a generation", GH_ParamAccess.item, 10); // 2
            pManager.AddIntegerParameter("Generations", "gen", "Number of generations", GH_ParamAccess.item, 5); // 3
            pManager.AddIntegerParameter("Seed", "sd", "Seed for Random", GH_ParamAccess.item, 0); // 4
            pManager.AddBooleanParameter("Run optimization", "run", "Set input to true for the component to run", GH_ParamAccess.item); //5
            pManager.AddBooleanParameter("Reset Solver", "reset", "Press the button to clear the results and reset the solver", GH_ParamAccess.item); // 6

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
            SH_SimpleShape ss = new SH_SimpleShape(); // 0
            List<SH_Rule> rulesList = new List<SH_Rule>(); // 1
            bool run = false;
            bool reset = false;
            // --- input ---
            if (!DA.GetData(0, ref ss)) return; // 0
            if (!DA.GetDataList(1, rulesList)) return;  // 1
            DA.GetData(2, ref populationSize); // 2
            DA.GetData(3, ref generations); // 3
            DA.GetData(4, ref Seed); // 4
            if (!DA.GetData(5, ref run)) return; // 5
            DA.GetData(6, ref reset); // 6

            maxEvals = populationSize * generations; // total number of evaluations

            if (reset)
            {
                //mooDone = true;
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Solution is reset.");
                ObjectiveVariables = new List<List<SH_Rule>>(); // reset list of variables
                ObjectiveValues = new List<List<double>>(); // reses list of values
                var runBool = (GH_BooleanToggle)this.Params.Input[5].Sources[0];
                if (runBool != null) { runBool.Value = false; }
                run = false;
                mooDone = false;
            }

            //Create a deep copy of the simple Shape before performing rule operations
            SH_SimpleShape copyShape = SH_UtilityClass.DeepCopy(ss);
            SimpleShape = copyShape; // assign the SH_SimpleShape instance to the component's propery

            // Control the input
            if (populationSize % 2 == 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The population size must be an even number.");
                return;
            }
            

            if (!run && !mooDone) // Make sure to include the "mooDone" here to avoid a complete rerun if the user refresh the solution
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Set boolean to \"True\" to run the component.");
            }

            if (Seed != 0) { MyRand = new Random(Seed); } // reset Random to give same result each time

            // --- solve ---

            DataTree<SH_Rule> genomeTree = new DataTree<SH_Rule>();
            DataTree<double> objValTree = new DataTree<double>(); 
            if (run)
            {
                // instantiate the solver
                GrammarRules = new List<object>() { "SH_Rule01", "SH_Rule02", "SH_Rule03", "SH_RuleA" }; // select the available rules for the interpreter
                GrammarWeights = new List<double>() { 0.1, 0.6, 0.15, 0.15 }; // the list of probabilities for each rule to be selected
                GrammarObjectives = new List<string>() { "max displacement", "total mass" };

                SH_NSGAIIProblem problem = new SH_NSGAIIProblem("SH_Solution", this, solutionsCounter);
                SH_NSGAIIRunner runner = new SH_NSGAIIRunner(null, problem, null, this);
                var allSolutions = problem.allSolutions;
                var paretoSolutions = allSolutions.GetRange(allSolutions.Count - 1 - populationSize, populationSize);

                GetDataTreesFromResults(paretoSolutions, out genomeTree, out objValTree); 
                
                // set the "Run optimisation" boolean input to False for user safety. 
                var runBool = (GH_BooleanToggle) this.Params.Input[5].Sources[0];
                if (runBool != null) { runBool.Value = false;}

                mooDone = true; 
            }
            

            DA.SetDataTree(2, genomeTree);
            DA.SetDataTree(3, objValTree);
            

        }

        #region Methods
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

        private void GetDataTreesFromResults(List<(List<SH_Rule> genome, List<double> fitness)> allSolutions, out DataTree<SH_Rule> genomes, out DataTree<double> objFitnessValues)
        {
            DataTree<SH_Rule> tree1 = new DataTree<SH_Rule>(); // tree for list of rules
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