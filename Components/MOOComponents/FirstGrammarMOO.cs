using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;
using Grasshopper.Kernel.Special;



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
            ObjectiveVariables = new List<SH_Rule>();
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
        public bool mooDone;
        public List<List<double>> ObjectiveValues;
        public List<SH_Rule> ObjectiveVariables;
        public List<double> objectives;
        public List<object> GrammarRules;
        public List<double> GrammarWeights; 
        public List<GH_NumberSlider> slidersList = new List<GH_NumberSlider>();
        public Random MyRand;
        public int Seed;
        public string log = null;
        private ObjectiveComparer comparer;
        public int populationSize = 0, generations = 0, maxEvals = 0;
        



        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Rules", "rules", "List of rules applicable for the grammar", GH_ParamAccess.list); // 0
            pManager.AddIntegerParameter("Population", "pop", "Number of individuals in a generation", GH_ParamAccess.item, 10); // 1
            pManager.AddIntegerParameter("Generations", "gen", "Number of generations", GH_ParamAccess.item, 5); // 2
            pManager.AddIntegerParameter("Seed", "sd", "Seed for Random", GH_ParamAccess.item, 0); // 3
            pManager.AddBooleanParameter("Run optimisation", "run", "Set input to true for the component to run", GH_ParamAccess.item, false); //4
            pManager.AddBooleanParameter("Reset Solver", "reset", "Press the button to clear the results and reset the solver", GH_ParamAccess.item); // 5

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Grammar derivation", "lst", "The rules applicable for the grammar derivation", GH_ParamAccess.tree); // 0
            pManager.AddGenericParameter("Objectives", "obj", "Objective values from each generation", GH_ParamAccess.tree); // 1
            pManager.AddGenericParameter("Test output", "test", "", GH_ParamAccess.list); // 2
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            List<SH_Rule> rulesList = new List<SH_Rule>(); // 0
            bool run = false;
            bool reset = false;
            // --- input ---
            if (!DA.GetDataList(0, rulesList)) return;  // 0
            DA.GetData(1, ref populationSize); // 1
            DA.GetData(2, ref generations); // 2
            DA.GetData(3, ref Seed); // 3
            DA.GetData(4, ref run); // 4
            DA.GetData(5, ref reset); // 5

            maxEvals = populationSize * generations; // total number of evaluations

            // Control the input
            if (populationSize % 2 == 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The population size must be an even number.");
                return;
            }
            
            if (reset)
            {
                //mooDone = true;
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Solution is reset.");
                ObjectiveVariables = new List<SH_Rule>(); // reset list of variables
                ObjectiveValues = new List<List<double>>(); // reses list of values
            }

            if (!run && !mooDone) // Make sure to include the "mooDone" here to avoid a complete rerun if the user refresh the solution
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Set boolean to \"True\" to run the component.");
            }

            if (Seed != 0) { MyRand = new Random(Seed); } // reset Random to give same result each time

            // --- solve ---

            if (run)
            {
                // initiate solver
                GrammarRules = new List<object>() { "SH_Rule01", "SH_Rule02", "SH_Rule03", "SH_RuleA" };
                GrammarWeights = new List<double>() { 0.1, 0.5, 0.3, 0.1 };
                Random rnd = MyRand; // instantiate a random instance
                List<SH_Rule> ruleList = MOO_Utility.NewGenome(GrammarRules, GrammarWeights, rnd);

                ObjectiveVariables = ruleList;
                mooDone = true; 
            }
            

            DA.SetDataList(2, ObjectiveVariables);
            

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