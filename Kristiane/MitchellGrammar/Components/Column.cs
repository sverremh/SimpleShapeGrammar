using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SimpleShapeGrammar.Kristiane.MitchellGrammar
{
    public class Column : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SubStructure class.
        /// </summary>
        public Column()
          : base("Rule_Column", "Nickname",
              "Defining Column",
              "SimpleGrammar", "Kristiane")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //pManager.AddNumberParameter("Number of substructure","nrSub","Gives the number of the substructures",GH_ParamAccess.item,0) ;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Mitchell Rule Class 6", "MRule6", "The Rule to be applied in the Shape Grammar", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // variables

            //input
            //if (!DA.GetData(0, ref nrSub)) return;


            // solve
            ColumnRule MRule2 = new ColumnRule();

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
            get { return new Guid("06690E33-B184-48F2-87A7-541A5E0BA676"); }
        }
    }
}