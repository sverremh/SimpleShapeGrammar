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
            pManager.AddGenericParameter("SH_Elements", "elems", "SH_Elements", GH_ParamAccess.list); // 0
            pManager.AddGenericParameter("SH_Supports", "sup", "SH_Supports", GH_ParamAccess.list); // 1

            pManager[1].Optional = true;
            // future implementation below


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
            List<SH_Support> sups = new List<SH_Support>();

            // future implementation below

            // List<SH_Load> loads = new List<SH_Load>();

            // --- input ---
            if (!DA.GetDataList(0, elems)) return;
           
            if(!DA.GetDataList(1, sups))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "There are no supports in the assembly!");
            }

            // deep copy the input
            elems = SH_UtilityClass.DeepCopy(elems);
            sups = SH_UtilityClass.DeepCopy(sups);
            SH_SimpleShape simpleShape = new SH_SimpleShape();

            // --- solve ---

            
            // renumbering Element Ids
            simpleShape.elementCount = 0;
            // renumbering Node IDs
            simpleShape.nodeCount = 0;
            simpleShape.supCount = 0;


            
            List<SH_Node> nodes = new List<SH_Node>();
            List<SH_Element> numberedElems = new List<SH_Element>();
            foreach (SH_Element e in elems)
            {
                e.ID = simpleShape.elementCount;
                simpleShape.elementCount++;
                numberedElems.Add(e);
                //simpleShape.Elements.Add(e);

                // node check and renumbering
                foreach (SH_Node node in e.Nodes)
                {
                    // test if there is already a node in this position
                    if (nodes.Any(n => n.Position.DistanceToSquared(node.Position) < 0.001 ))
                    {                        
                        continue;
                    }
                    

                    node.ID = simpleShape.nodeCount;
                    nodes.Add(node);
                    simpleShape.nodeCount++;

                }
            }
            simpleShape.Elements = numberedElems;

            // add supports to the element
            List<SH_Support> uniqueSupports = new List<SH_Support>();
            foreach (var sup in sups)
            {
                if (uniqueSupports.Any(s => s.Position.DistanceToSquared(sup.Position) < 0.001))
                {
                    // if there is already a support at this position it is not added
                    continue;
                }

                // find the index of the node where the support applies
                int nodeInd = nodes.FindIndex( n => n.Position.DistanceToSquared(sup.Position) < 0.001 );
                if (nodeInd != -1) // if -1 the location of the support don't match a node
                {
                    sup.ID = simpleShape.supCount++;
                    sup.nodeInd = nodeInd;
                    uniqueSupports.Add(sup);
                    simpleShape.supCount++;
                }
                
            }

            




            simpleShape.Nodes = nodes;
            simpleShape.Supports = uniqueSupports;
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