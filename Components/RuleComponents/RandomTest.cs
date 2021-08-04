using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;
namespace SimpleShapeGrammar.Components
{
    public class RandomTest : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RandomTest class.
        /// </summary>
        public RandomTest()
          : base("RandomTest", "Nickname",
              "Description",
              "SimpleGrammar", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Lenght", "len", "The length of the list of rules to be generated.", GH_ParamAccess.item, 5); // 0
            
            // not implemented
            // - weights as list
            // - rules as just classes? Not sure how to solve this best yet.
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("SH_Rules", "rules", "List of rules to input in the interpreter.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
           
            int length = 0;

            // --- input ---
            DA.GetData(0, ref length);
            int numLines = 1; // in the beginning there is just one line
            // --- solve ---

            List<SH_Rule> rulesList = new List<SH_Rule>(); // initiate empty list
            Random random = new Random(Guid.NewGuid().GetHashCode()); // create a guid and use the HashCode as seed for the random instance
            List<object> rules = new List<object>() { "r1", "r2", "rA" };
            List<double> weights = new List<double>() {0.1, 0.8, 0.1 };
            // loop to generate the rules
            for (int i = 0; i < length; i++)
            {
                // pick a rule randomly
                SH_UtilityClass.TakeRandomItem(rules, weights, random, out object rule);

                if (rule == rules[0])
                {
                    rulesList.Add(NewRule01(random));
                }
                else if (rule == rules[1] && rulesList.Count >= 1)
                {
                    rulesList.Add(NewRule02(random, ref numLines));
                }
                else if (rule == rules[2])
                {
                    rulesList.Add(NewRuleA());
                    break; // no rules are added after this state changer. 
                }
            }

            // --- output ---
            DA.SetDataList(0, rulesList);
        }
        private SH_Rule01 NewRule01(Random rnd)
        {
            // create start and end vector vector3d
            var v1 = new Vector3d(0, 0, SH_UtilityClass.RandomExtensions.NextDouble(rnd, -1.0, 1.0));
            var v2 = new Vector3d(0, 0, SH_UtilityClass.RandomExtensions.NextDouble(rnd, -2.0, 2.0));
            return new SH_Rule01(v1, v2);
        }
        private SH_Rule02 NewRule02(Random rnd, ref int numLines)
        {
            int ind = rnd.Next(0, numLines);
             // double check if this will include the final item in the list. 
            double param = SH_UtilityClass.RandomExtensions.NextDouble(rnd, 0.1, 0.9);
            var r2 = new SH_Rule02(ind, param);
            numLines += 1;
            return r2;
        }
        private SH_RuleA NewRuleA()
        {
            return new SH_RuleA();
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
            get { return new Guid("5185c4c3-6e07-495e-8e86-cb31c7245028"); }
        }
    }
}