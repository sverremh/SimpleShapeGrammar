using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SimpleShapeGrammar.Kristiane.MitchellGrammar
{
    public class SecondaryRoofStructure : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SecondaryRoofStructure class.
        /// </summary>
        public SecondaryRoofStructure()
         : base("Rule_SecondaryRoofStructure", "Nickname",
             "Generates secondary roof structure",
             "SimpleGrammar", "Kristiane")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Number of Secondary Roof Structure", " nrSecondaryRS", "Number to select type of Secondary Roof Structure", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Height", " h", "Height for the bowed and pitched secondary roof structure", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("Count", " cnt", "Number of segments for secondary roof structure", GH_ParamAccess.item, 6);
            pManager.AddIntegerParameter("Amount of Secondary Roof Structure", " numberSecondaryRS", "How many Secondary Roof Structures", GH_ParamAccess.item, 2);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Mitchell Rule Class 4", "MRule4", "The Rule to be applied in the Shape Grammar", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // variables
            int nrSecondaryRS = 0; //number of secondary roof strcuture for substructure 0 and 1
            double h = 0; //height for pitched and bowed secondary roof structure
            int count = 0; //number of segments for secondary roof structure
            int numberSecondaryRS = 0; //Amount of secondary roof structure elements


            //input
            if (!DA.GetData(0, ref nrSecondaryRS)) return;
            if (!DA.GetData(1, ref h)) return;
            if (!DA.GetData(2, ref count)) return;
            if (!DA.GetData(3, ref numberSecondaryRS)) return;

            // solve
            SecondaryRoofStructureRule MRule4 = new SecondaryRoofStructureRule(nrSecondaryRS, h, count, numberSecondaryRS);

            // output
            DA.SetData(0, MRule4);
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
            get { return new Guid("B3687A93-FC8C-4259-BE2B-138B9208F2EC"); }
        }
    }
}