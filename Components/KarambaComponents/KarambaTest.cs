using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Karamba.GHopper.Models;
using Karamba.Models;
using Karamba.Utilities;
using Karamba.Geometry;
using Karamba.CrossSections;
using Karamba.Supports;
using Karamba.Loads;


namespace ShapeGrammar.Components
{
    public class KarambaTest : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the KarambaTest class.
        /// </summary>
        public KarambaTest()
          : base("Karamba Analysis Th.I", "Karamba Th.I",
              "Karamba Analysis Th.I",
              "SimpleGrammar", "Karamba")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // pManager.AddParameter(new Param_Model(), "Model_in", "Model_in",
            //          "Model to be manipulated", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Model_out" ,"mod", "Model of simple beam", GH_ParamAccess.item);
            //pManager.RegisterParam(new Param_Model(), "Model_out", "Model_out",  "Model after eliminating all tension elements");
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

            // --- input --- 



            // --- solve ---

            // points
            var p0 = new Point3(0, 0, 0);
            var p1 = new Point3(0, 0, 10);
            var L0 = new Line3(p0, p1);

            // material

            // crosecs

            //var crosec = new Karamba.CrossSections.CroSec_Trapezoid(
            //    "", "Rect", "EN", System.Drawing.Color.HotPink, new Karamba.Materials.FemMaterial_Isotrop(), 30, 20, 20);
            // var crosecs = new List<CroSec>() { crosec };

            // elems
            var elems = k3d.Part.LineToBeam(new List<Line3>() { L0 },
                new List<string>() { "C1" },
                new List<CroSec>(), logger, out nodes);

            // supports
            var cond = new List<bool>() { true, true, true, true, true, true };
            var support = k3d.Support.Support(0, cond);
            var supports = new List<Support>() { support };

            // loads
            var pload = k3d.Load.PointLoad(1,
                new Vector3(10, 0, 0), new Vector3());
            var ploads = new List<Load>() { pload };
            

            // assembly
            double mass;
            Point3 cog;
            bool flag;
            string info;
            var model = k3d.Model.AssembleModel(elems, supports, ploads,
              out info, out mass, out cog, out info, out flag);

            // calculate Th.I response
            List<double> max_disp;
            List<double> out_g;
            List<double> out_comp;
            string message;
            model = k3d.Algorithms.AnalyzeThI(model, out max_disp, out out_g, out out_comp, out message);
            var out_model = new Karamba.GHopper.Models.GH_Model(model);
            // --- output ---
            //DA.SetData(0, max_disp[0]);
            DA.SetData(0, out_model);
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
            get { return new Guid("9579765a-0fa2-4556-b139-8842ff640b0b"); }
        }
    }
}