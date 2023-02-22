using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;

using System.Linq;
using SimpleShapeGrammar.Classes.Elements;

namespace SimpleShapeGrammar.Components
{
    // This assembly component is not yet compatibel with other geometries than lines as for the simple bridge and truss roof grammar. 
    [Serializable]
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
            pManager.AddGenericParameter("SH_Loads", "loads", "SH_Loads", GH_ParamAccess.list); // 2

            // make the support and load parameters optional
            pManager[1].Optional = true;
            pManager[2].Optional = true;

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
            List<SH_Load> loads = new List<SH_Load>();
            

            // --- input ---
            if (!DA.GetDataList(0, elems)) return;
           
            if(!DA.GetDataList(1, sups))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "There are no supports in the assembly!");
            }

            if(!DA.GetDataList(2, loads))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "There are no loads in the assembly!");
            }

            // deep copy the input
            elems = SH_UtilityClass.DeepCopy(elems); // How to get this into the form of Dictionary? Multiple input? 
            sups = SH_UtilityClass.DeepCopy(sups);
            loads = SH_UtilityClass.DeepCopy(loads);
            var curves = new List<NurbsCurve>();

            SH_SimpleShape simpleShape = new SH_SimpleShape();

            // --- solve ---
            simpleShape.NurbsCurves = curves;
            
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
                    if (nodes.Any(n => n.Pt.DistanceToSquared(node.Pt) < 0.001 ))
                    {                        
                        continue;
                    }
                    
                    node.ID = simpleShape.nodeCount;
                    nodes.Add(node);
                    simpleShape.nodeCount++;

                }
            }
            simpleShape.Elems = numberedElems;

            
            /*
            foreach (var sup in sups)
            {
                

                // there are only two supports in this grammar. Append the supports to end points of initial line


                // find the index of the node where the support applies
                int nodeInd = nodes.FindIndex( n => n.Position.DistanceToSquared(sup.Position) < 0.001 );
                if (nodeInd != -1) // if -1 the location of the support don't match a node
                {
                    sup.ID = simpleShape.supCount;
                    simpleShape.supCount++;
                    sup.nodeInd = nodeInd;
                    uniqueSupports.Add(sup);
                }
                
            }
            */
            // add the loads to the simple shape            
            SortLoads(loads, out List<SH_LineLoad> l_loads, out List<SH_PointLoad> p_loads);

            



            simpleShape.Nodes = nodes;


            // add supports to the simple shape
            List<SH_Support> uniqueSupports = new List<SH_Support>();
            foreach (SH_Node node in simpleShape.Nodes)
            {
                if (uniqueSupports.Any(s => s.Position.DistanceToSquared(node.Pt) < 0.001))
                {
                    // if there is already a support at this position it is not added
                    continue;
                }

                // find the support at the node. 
                var nodeSup = sups.Find(s => s.Position.DistanceToSquared(node.Pt) < 0.01);
                nodeSup.ID = simpleShape.supCount;
                simpleShape.supCount++;
                nodeSup.nodeInd = (int)node.ID;

                uniqueSupports.Add(nodeSup);
            }

            simpleShape.LineLoads = l_loads;
            simpleShape.PointLoads = p_loads;
            simpleShape.Supports = uniqueSupports;
            simpleShape.SimpleShapeState = State.alpha;

            // --- output ---
            DA.SetData(0, simpleShape); 


        }
        /// <summary>
        /// Private methods for this components
        /// </summary>
        private void SortLoads(List<SH_Load> loads ,out List<SH_LineLoad> line_loads, out List<SH_PointLoad> point_loads)
        {
            // create the empty list
            List<SH_PointLoad> pl = new List<SH_PointLoad>();
            List<SH_LineLoad> ll = new List<SH_LineLoad>();

            // iterate through all the loads
            foreach (var l in loads)
            {
                if (l is SH_LineLoad)
                {
                    ll.Add( (SH_LineLoad) l);
                }
                if (l is SH_PointLoad)
                {
                    pl.Add( (SH_PointLoad) l);
                }
            }
            line_loads = ll;
            point_loads = pl;
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
                return ShapeGrammar.Properties.Resources.icons_C_Mdl;
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