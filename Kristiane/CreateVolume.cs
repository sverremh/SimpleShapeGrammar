using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;
using Grasshopper.Kernel;
using Rhino.Geometry;
using SimpleShapeGrammar.Classes.Elements;
namespace SimpleShapeGrammar.Kristiane.MitchellGrammar
{
    public class CreateVolume : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SubStructure class.
        /// </summary>
        public CreateVolume()
          : base("CreateVolume", "volume",
              "Create geometry from Simple Shape",
              "SimpleGrammar", "Kristiane")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Simple Shape", "sShape", "The Simple Shape element", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "b", "Brep from the simple shape", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // variables
            SH_SimpleShape simpleShape = new SH_SimpleShape();

            //input
            //DA.GetData(0, ref simpleShape);
            DA.GetData(0, ref simpleShape);


            // solve
            List<Brep> breps = new List<Brep>();

            var elLines = simpleShape.Elements["Line"];
            foreach (SH_Line sh_line in elLines)
            {
                // Create Start point
                Point3d sPt = sh_line.Nodes[0].Position;
                // Create End point
                Point3d ePt = sh_line.Nodes[1].Position;

                Line line = new Line(sPt, ePt);
                Curve crv = line.ToNurbsCurve();
                

                string matName = sh_line.CrossSection.Material.Name;

                //Accecc crossSec and construct volume/Brep
                string cSecName = sh_line.CrossSection.Name;

                //Get dimensions
                string[] dimString = cSecName.Split('x');
                double h = Convert.ToDouble(dimString.GetValue(0))*0.001;
                double w = Convert.ToDouble(dimString.GetValue(1))*0.001;
                
                //Construct plane
                Vector3d vec = line.UnitTangent;
                Plane plane = new Plane(sPt, vec);

                //Interval for cross-section
                Interval width = new Interval(-w / 2, w / 2);
                Interval height = new Interval(-h / 2, h / 2);

                // Cross-section (rec) and Curve to sweep along to get a volume
                Rectangle3d cSec = new Rectangle3d(plane, width, height);
                Curve crossSec = cSec.ToNurbsCurve();
                Brep[] brep = Brep.CreateFromSweep(crv, crossSec, false, 0.001);
                breps.AddRange(brep);
            }

            // output
            DA.SetDataList(0, breps);
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
            get { return new Guid("06690E33-B184-48F2-87A7-541A5E0BA679"); }
        }
    }
}