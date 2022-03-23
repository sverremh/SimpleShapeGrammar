using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;
using Grasshopper.Kernel;
using Rhino.Geometry;


namespace SimpleShapeGrammar.Kristiane.MitchellGrammar
{
    public class BrepToSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BrepToSurface class.
        /// </summary>
        public BrepToSurface()
          : base("Rule_BrepToSurface", "Nickname",
              "Exploding brep into surfaces",
              "SimpleGrammar", "Kristiane")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //pManager.AddBrepParameter("Brep", "B", "Input geometry is a Brep", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Mitchell Rule Class 1", "MRule1", "The Rule to be applied in the Shape Grammar", GH_ParamAccess.item);
            //pManager.AddSurfaceParameter("Surfaces", "SrfLst", "List of Surfaces", GH_ParamAccess.list);
            //pManager.AddTextParameter("Surface Name", "SrfName", "List with name of Surfaces", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // variables
            //Brep geo = new Brep();

            // inputs
            //if (!DA.GetData(0, ref geo)) return;

            // solve
            BrepToSurfaceRule MRule1 = new BrepToSurfaceRule();

            // output
            DA.SetData(0, MRule1);

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
            get { return new Guid("74EECE0E-63E4-46C5-8C92-45B62D80857E"); }
        }
    }
}