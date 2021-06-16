using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;
namespace SimpleShapeGrammar.Components
{
    public class GrammarInterpreter : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GrammarInterpreter class.
        /// </summary>
        public GrammarInterpreter()
          : base("GrammarInterpreter", "interpreter",
              "Description",
              "SimpleGrammar", "Interpreter")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Simple Shape", "sShape", "Simple Shape to be modified with the rules", GH_ParamAccess.item);
            pManager.AddGenericParameter("Rules", "rls", "Rules to apply to the Interpreter", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Modified Shape", "mShape", "Shape Class after Grammar derivation", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            SH_SimpleShape simpleShape = new SH_SimpleShape();
            List<SH_Rule> rules = new List<SH_Rule>();

            // --- input --- 
            if (!DA.GetData(0, ref simpleShape)) return;
            if (!DA.GetDataList(1, rules)) return;

            //Create a deep copy of the simple Shape before performing rule operations
            SH_SimpleShape copyShape = SH_UtilityClass.DeepCopy(simpleShape);

            // --- solve ---
            foreach (SH_Rule rule in rules)
            {
                rule.RuleOperation(copyShape);
            }
            
            // --- output ---
            DA.SetData(0, copyShape);
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
            get { return new Guid("6f3252a6-31bb-4d33-9123-447465a8185b"); }
        }
    }
}