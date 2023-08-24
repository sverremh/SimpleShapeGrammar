using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Karamba.GHopper.Models;
using Karamba.Models;
using Karamba.Utilities;
using Karamba.Elements;
using Karamba.Geometry;
using Karamba.GHopper.Geometry;
using Karamba.CrossSections;
using Karamba.Supports;
using Karamba.Loads;
using Karamba.Factories;
using ShapeGrammar.Classes;
using ShapeGrammar.Classes.Elements;


namespace ShapeGrammar.Components
{
    public class SH_ShapeToKarambaModel : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SH_ShapeToKarambaModel class.
        /// </summary>
        public SH_ShapeToKarambaModel()
          : base("SH_ShapeToKarambaModel", "SH_2_K3d",
              "Converts the SH_SimpleShape to a Karamba Model",
              "SimpleGrammar", "Karamba")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("SH_SimpleShape", "SH_SS", "An instance of a simple shape class", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Model_out", "m_out", "Karamba3D model", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---      
            var nodes = new List<Point3>();
            var logger = new MessageLogger();
            var k3d = new KarambaCommon.Toolkit();
            var ss = new SG_Shape();


            // --- input ---
            if (!DA.GetData(0, ref ss)) return; // retrieve the simpleshape


            // --- solve ---

            // create karamba Line3 elements
            List<string> element_names;
            List<Line3> k_lines = SH_ElementsToKarambaLines(ss.Elems, k3d, out element_names);

            // create Karamba Builder Beams from Line3 list. 
            List<BuilderBeam> elems = k3d.Part.LineToBeam(k_lines, element_names,
                new List<CroSec>(), logger, out nodes);
            /// <summary>
            /// not implemented: The cross section which should also come from the SH_SimpleElement.
            /// </summary>


            // supports
            // not implemented yet. Read through all the nodes. Create a support instance if the condition theres is not 0
            var supports = new List<Support>();
            foreach (var sup in ss.Supports)
            {
                // karamba point
                Point3 loc = new Point3(sup.Node.Pt.X, sup.Node.Pt.Y, sup.Node.Pt.Z);
                

                // conditions
                List<bool> conditions = CreateBooleanConditions(sup.SupportCondition);

                // not implemented: Optional Plane

                // create support
                Support gh_sup = k3d.Support.Support(loc, conditions);
                supports.Add(gh_sup);
            }



            // loads

            ///<summary>
            ///Note: Neither gravity load nor line load has a k3d (factory) initatior. 
            /// </summary>

            var loads = new List<Load>();
            // gravity load
            var gLoad = new GravityLoad(new Vector3(0, 0, -1), "1");
            loads.Add(gLoad);

            // line loads
            var lineLoads = new List<Load>();
            foreach(var l in ss.LineLoads)
            {
                //var ids = l.ElementIds;
                var ids = l.ElementId; 
                var k_vec = new Vector3(l.Load.X, l.Load.Y, l.Load.Z);
                var orient = LoadOrientation.global;
                int lc = l.LoadCase;
                var k_lineLoad = k3d.Load.ConstantForceLoad(k_vec.Unitized, k_vec.Length, 0.0, 1.0, orient, lc.ToString(), ids);
                // need to make a new beam load...
                //var k_lineLoad = new UniformlyDistLoad(ids, k_vec, orient, lc.ToString());
                // var k_lineLoad = new Karamba.Loads.Beam.DistributedForce()
                lineLoads.Add(k_lineLoad);
            }

            loads.AddRange(lineLoads);
            

            // point loads : not implemented yet
            
            
            

            // assembly
            double mass;
            Point3 cog; // centre of gravity for the model
            bool flag;
            string info;
            Model model = k3d.Model.AssembleModel(elems, supports, loads,
                out info, out mass, out cog, out info, out flag);


            // create GH_Model wrapper to enable pipelining of the model
            var out_model = new Karamba.GHopper.Models.GH_Model(model);

            // --- output ---
            DA.SetData(0, out_model);
        }

        /// <summary>
        /// Functions used in the component
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="k3d"></param>
        /// <returns></returns>
        private List<Line3> SH_ElementsToKarambaLines(List<SG_Element> elements, KarambaCommon.Toolkit k3d, out List<string> el_names)
        {
            // initiate list
            
            List<Line3> k_lines = new List<Line3>();
            List<string> k_names = new List<string>();
            // create karamabe BuilderBeam elements using Factory method
            foreach (SG_Element el in elements)
            {                
                // get node points
                Point3d sPt = el.Nodes[0].Pt;
                Point3d ePt = el.Nodes[1].Pt;
                // convert to karamba's Point3

                Point3 k_sPt = new Point3(sPt.X, sPt.Y, sPt.Z);
                Point3 k_ePt = new Point3(ePt.X, ePt.Y, ePt.Z);
                

                // create Line3
                Line3 k_line = new Line3(k_sPt, k_ePt);
                k_lines.Add(k_line);

                // add name
                k_names.Add(el.Name);
                
                
            }
            el_names = k_names;
            return k_lines;
        }
        private List<bool> CreateBooleanConditions(int condInt)
        {
            List<bool> conditions = new List<bool>() { false, false, false, false, false, false };
            int val = condInt;
            for (int i = 5; i >= 0; i--)
            {
                int rest = (int)Math.Pow(2, i);
                if (val - rest >= 0)
                {
                    conditions[i] = true;
                    val -= rest;
                }
            }
            return conditions;
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
            get { return new Guid("47df4430-7821-4821-b7ec-3e5b5f9df7ab"); }
        }
    }
}