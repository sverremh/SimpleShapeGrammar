using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SimpleShapeGrammar.Kristiane.MitchellGrammar
{
    public class SubStructure : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SubStructure class.
        /// </summary>
        public SubStructure()
          : base("Rule_SubStructure", "Nickname",
              "Defining substructures",
              "SimpleGrammar", "Kristiane")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Number of substructure", "nrSub", "Gives the number of the substructures", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Height", "h", "Height for pitched and bowed roof", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Count", "cnt", "Count to divide arhc, bowed roof", GH_ParamAccess.item, 6);
            pManager.AddTextParameter("Cross-Section", " cSec", "Cross-Section", GH_ParamAccess.item, "200x200");
            pManager.AddTextParameter("Material", " matName", "Material Name", GH_ParamAccess.item, "timber_C20");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Mitchell Rule Class 2", "MRule2", "The Rule to be applied in the Shape Grammar", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // variables
            double nrSub = 0; //number of subsection
            double h = 0; // height
            double count = 0; //count, divide arch for bowed roof
            string cSec = "200x200"; // cross-section
            string matName = "timber_C20"; //material

            //input
            if (!DA.GetData(0, ref nrSub)) return;
            if (!DA.GetData(1, ref h)) return;
            if (!DA.GetData(2, ref count)) return;
            if (!DA.GetData(3, ref cSec)) return;
            DA.GetData(4, ref matName);


            // solve
            SH_Material beamMat = new SH_Material(matName);
            SubStructureRule MRule2 = new SubStructureRule(nrSub, h, count, cSec, beamMat);

            // output
            DA.SetData(0, MRule2);

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
            get { return new Guid("06690E33-B184-48F2-87A7-541A5E0BAF9F"); }
        }
    }
}