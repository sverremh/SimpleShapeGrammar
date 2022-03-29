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
          : base("Rule_PrimaryRoofStructure", "Nickname",
              "Generates primary roof structure",
              "SimpleGrammar", "Kristiane")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Number of Primary Roof Structure", " nrPrimaryRS", "Number to select type of Primary Roof Structure", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Height", " hBeam", "Beam height for for primary roof structure for substructure 0 and 1.", GH_ParamAccess.item,1);
            pManager.AddNumberParameter("Count", " Divisions", "Divisions of truss beam for substructure 0 and 1.", GH_ParamAccess.item,4);
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
            double nrPrimaryRS = 0; //number of primary roof strcuture for substructure 0 and 1
            double hBeam = 0; // height beam
            double count = 0; // divisions for truss

            //input
            if (!DA.GetData(0, ref nrPrimaryRS)) return;
            if (!DA.GetData(1, ref hBeam)) return;
            if (!DA.GetData(2, ref count)) return;

            // solve
            PrimaryRoofStructureRule MRule3 = new PrimaryRoofStructureRule(nrPrimaryRS, hBeam, count);

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