using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;

namespace SimpleShapeGrammar.Components
{
    public class Assembly : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Assembly class.
        /// </summary>
        public Assembly()
          : base("Assembly", "Nickname",
              "Description",
              "SimpleGrammar", "Assembly")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("Initial Line", "initLine", "Line to be used in the simple grammar derivaiton.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Simple Shape instance", "sShape", "In instance of the Simple Shape class.", GH_ParamAccess.item); 
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            Line line = new Line();

            // --- input --- 
            if (!DA.GetData(0, ref line)) return;

            // --- solve ---

            //Initiate the Simple Shape
            SH_SimpleShape simpleShape = new SH_SimpleShape();

            // Create the SH_Node and SH_Lines. 
            SH_Node[] nodes = new SH_Node[2];
            nodes[0] = new SH_Node(line.FromX, line.FromY, line.FromZ);
            nodes[1] = new SH_Node(line.ToX, line.ToY, line.ToZ);

            SH_Line sH_Line = new SH_Line(nodes);


            // --- output ---
            DA.SetData(0, simpleShape);

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
            get { return new Guid("beaaf8ac-603a-49bf-9a4b-39ce573c5f44"); }
        }
    }
}