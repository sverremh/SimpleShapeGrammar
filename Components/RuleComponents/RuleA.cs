﻿using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using ShapeGrammar.Classes;
using ShapeGrammar.Classes.Rules;

namespace ShapeGrammar.Components
{
    public class RuleA : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RuleA class.
        /// </summary>
        public RuleA()
          : base("RuleA", "rulea",
              "Change the state of the system to gamma",
              "SimpleGrammar", "Rules")
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
            pManager.AddGenericParameter("RuleA Class", "ruleA", "Generate RuleA", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---

            // --- input --- 

            // --- solve ---
            SH_RuleA ruleA = new SH_RuleA();

            // --- output ---
            DA.SetData(0, ruleA);
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
            get { return new Guid("0bd2e8c7-e2a6-471a-bd78-54ed0c1b7ff1"); }
        }
    }
}