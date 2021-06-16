using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;

namespace SimpleShapeGrammar.Components
{
    public class LineToElement : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Assembly class.
        /// </summary>
        public LineToElement()
          : base("LineToElement", "lnToEl",
              "Creates a SH_Element from a Line",
              "SimpleGrammar", "Element")
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
            pManager.AddGenericParameter("SH_Element", "sH_el", "An instance of a SH_Element", GH_ParamAccess.item); 
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

            // Restart the counter
            

            //Initiate the Simple Shape
            SH_SimpleShape simpleShape = new SH_SimpleShape();

            // Create the SH_Node and SH_Lines. 
            SH_Node[] nodes = new SH_Node[2];
            nodes[0] = new SH_Node(line.From, null);
            nodes[1] = new SH_Node(line.To, null);
                   
            SH_Element sH_Line = new SH_Element(nodes, null);
            
          
            // --- output ---
            DA.SetData(0, sH_Line);

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
                return SimpleShapeGrammar.Properties.Resources.icons_C_Elem1D;
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