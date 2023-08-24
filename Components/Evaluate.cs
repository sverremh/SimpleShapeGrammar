using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using ShapeGrammar.Classes;
namespace ShapeGrammar.Components
{
    public class Evaluate : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Evaluate class.
        /// </summary>
        public Evaluate()
          : base("Evaluate", "eval",
              "Temporary component evaluating the forces",
              "SimpleGrammar", "Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("SS_SimpleShape", "", "", GH_ParamAccess.item); // 0
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("reactions", "moments", "List of moments over \"Supports\".", GH_ParamAccess.list); // 0
            pManager.AddGenericParameter("reciprocal", "reciprocal", "Reciprocal diagram", GH_ParamAccess.list); // 0
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            SG_Shape ss = new SG_Shape();

            // --- input ---
            if (!DA.GetData(0, ref ss)) return;

            // --- solve ---
            SH_Evaluation.ConstructMatrices(ss, out double[,] a, out double[] b);

            // calculate moments over the supports
            double[] moments = SH_Evaluation.CalculateMoments(a, b);
            double[] forces = SH_Evaluation.CalculateForces(ss, moments);
            double thrust = 50; // make this a user specified input later.
            double[] reactions = SH_Evaluation.CalculateReactions(ss, forces, thrust);


            // draw reciprocal diagram
            Dictionary<string, List<Line>> reciprocal_diagram = new Dictionary<string, List<Line>>();
            try
            {
                reciprocal_diagram = SH_Evaluation.DrawReciprocal(ss, reactions, forces, thrust);
                // --- output ---
                DA.SetDataList(0, reactions);
                DA.SetDataList(1, reciprocal_diagram["internal"]);
            }
            catch (Exception)
            {

                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not enough elements to draw reciprocal. Minimum number is 2.");
            }
            


            
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
            get { return new Guid("af06a762-48c9-4f99-aa12-74c3e3f20ea7"); }
        }
    }
}