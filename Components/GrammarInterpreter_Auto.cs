using System;
using System.Collections.Generic;

using Rhino.Geometry;
using Grasshopper.Kernel;

using ShapeGrammar.Classes;

namespace ShapeGrammar.Components
{
    public class GrammarInterpreter_Auto : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GrammerInterpreter_Auto class.
        /// </summary>
        public GrammarInterpreter_Auto()
          : base("GrammerInterpreter_Auto", "GI_Auto",
              "Automatic Grammar Interpreter",
              Util.CAT, Util.GR_INT)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Shape", "Shape", "Shape Grammar Assembly", GH_ParamAccess.item);
            pManager.AddGenericParameter("Automatic Rules", "Autorules", "Rules for Automatic Interpreter", GH_ParamAccess.list);
            pManager.AddGenericParameter("Genotype", "Genotype", "Genotype/Chromosome", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
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
            get { return new Guid("38d35ef6-a3b2-44b2-bfa7-23d1292d37f5"); }
        }
    }
}