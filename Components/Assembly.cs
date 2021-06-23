﻿using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;

using System.Linq;

namespace SimpleShapeGrammar.Components
{
    public class Assembly : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Assembly class.
        /// </summary>
        public Assembly()
          : base("Assembly", "Assembly",
              "Assembly",
              "SimpleGrammar", "Assembly")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("SH_Elements", "SH_Elements", "SH_Elements", GH_ParamAccess.list);
            
            // future implementation below
            
            // pManager.AddGenericParameter("SH_Supports", "SH_Supports", "SH_Supports", GH_ParamAccess.list);
            // pManager.AddGenericParameter("SH_Supports", "SH_Supports", "SH_Supports", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("SH_SimpleShape", "SH_SimpleShape", "SH_SimpleShape", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            List<SH_Element> elems = new List<SH_Element>();

            // future implementation below
            // List<SH_Support> sups = new List<SH_Support>();
            // List<SH_Load> loads = new List<SH_Load>();

            // --- input ---
            if (!DA.GetDataList(0, elems)) return;

            // deep copy the elements
            elems = SH_UtilityClass.DeepCopy(elems);
            SH_SimpleShape simpleShape = new SH_SimpleShape();

            // --- solve ---

            
            // renumbering Element Ids
            simpleShape.ElementCount = 0;
            // renumbering Node IDs
            simpleShape.NodeCount = 0;


            
            List<SH_Node> nodes = new List<SH_Node>();
            foreach (SH_Element e in elems)
            {
                e.ID = simpleShape.ElementCount;
                simpleShape.ElementCount++;

                simpleShape.Elements.Add(e);

                // node check and renumbering
                foreach (SH_Node node in e.Nodes)
                {
                    // test if there is already a node in this position
                    if (nodes.Any(n => n.Position.DistanceToSquared(node.Position) < 0.001 ))
                    {                        
                        continue;
                    }

                    node.ID = simpleShape.NodeCount;
                    nodes.Add(node);
                    simpleShape.NodeCount++;

                }

            }


            simpleShape.Nodes = nodes;
            simpleShape.SimpleShapeState = State.alpha;

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
                return SimpleShapeGrammar.Properties.Resources.icons_C_Mdl;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d3d9eb87-86c0-4891-9d50-6810495145af"); }
        }
    }
}