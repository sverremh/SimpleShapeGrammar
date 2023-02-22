using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using ShapeGrammar.Classes;

namespace ShapeGrammar.Components
{
    public class RandomRules : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RandomRules class.
        /// </summary>
        public RandomRules()
          : base("RandomRules", "random",
              "Generate a list of random rules from the input rules",
              "SimpleGrammar", "Rules")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Rules", "rules", "List of the rules that can be used", GH_ParamAccess.list); //0
            pManager.AddGenericParameter("Weights", "w", "Optional weights for the different rules. If none are provided, an equal distribution is chosen.", GH_ParamAccess.list); // 1
            pManager.AddIntegerParameter("RuleNumber", "num", "Number of random rules to be generated.", GH_ParamAccess.item, 5) ; // 2

            pManager[1].Optional = true; 

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("SH_Rules", "rules", "List of rules for interpreter.", GH_ParamAccess.list); // 0
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            List<object> rules = new List<object>();
            List<double> weights = new List<double>();
            int length = 0;


            // --- input ---
            if (!DA.GetDataList(0, rules)) return; 
            if(!DA.GetDataList(1, weights))
            {
                // if no input, create a list corresponding to the length of the rules list.
                //double w = (1 / rules.Count);
                double w = 0.33;
                for (int i = 0; i < rules.Count; i++)
                {
                    weights.Add(w);
                }

            }
            DA.GetData(2, ref length);


            // --- solve ---
            
            // iniate list of output rules
            List<object> out_rules = new List<object>();

            var rnd = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < length; i++)
            {
                Util.TakeRandomItem( rules, weights, rnd, out object rule);
                out_rules.Add(rule);
            }

            // --- output ---
            DA.SetDataList(0, out_rules);
        }

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
            get { return new Guid("04452294-e6cd-40eb-ac3a-458348ce3a44"); }
        }
    }
}