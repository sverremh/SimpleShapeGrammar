using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SimpleShapeGrammar.Kristiane.MitchellGrammar
{
    public class PrimaryRoofStructure : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PrimaryRoofStructure class.
        /// </summary>
        public PrimaryRoofStructure()
          : base("PrimaryRoofStructure", "Nickname",
              "Description",
              "SimpleGrammar", "Kristiane")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Mitchell Rule Class 3", "MRule3", "The Rule to be applied in the Shape Grammar", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // variables
            //double nrSub = 0; //number of subsection
            //double h = 0; // height
            //input
            //if (!DA.GetData(0, ref nrSub)) return;
            //if (!DA.GetData(1, ref h)) return;

            // solve
            PrimaryRoofStructureRule MRule3 = new PrimaryRoofStructureRule();

            // output
            DA.SetData(0, MRule3);
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
            get { return new Guid("CCFB04D1-E269-42FA-96C4-875F218EE865"); }
        }
    }
}