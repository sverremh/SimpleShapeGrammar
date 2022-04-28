using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SimpleShapeGrammar.Kristiane.MitchellGrammar
{
    public class LateralStability : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the LateralStability class.
        /// </summary>
        public LateralStability()
          : base("Rule_LateralStability", "Nickname",
              "Adds lateral stability",
              "SimpleGrammar", "Kristiane")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Number of Lateral Stability", "nrLatStability", "Number to select type of bracing.", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("Number of Wall", "nrWall", "Number to select wall to brace.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Corner distance", "distKneeBrace", "Distance from corner for knee brace", GH_ParamAccess.item, 0.1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Mitchell Rule Class 5", "MRule5", "The Rule to be applied in the Shape Grammar", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // variables
            int nrLateralStability = 0; //number of lateral stability
            int nrWall = 0; // number of wall to brace
            double distBrace = 0; //distance from corner for knee brace

            //input
            if (!DA.GetData(0, ref nrLateralStability)) return;
            if (!DA.GetData(1, ref nrWall)) return;
            if (!DA.GetData(2, ref distBrace)) return;

            // solve
            LateralStabilityRule MRule5 = new LateralStabilityRule(nrLateralStability, nrWall, distBrace);

            // output
            DA.SetData(0, MRule5);
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
            get { return new Guid("28EF03E3-6935-494E-B863-E8553BC2BF40"); }
        }
    }
}