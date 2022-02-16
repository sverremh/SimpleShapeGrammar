using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SimpleShapeGrammar.Kristiane.Components
{
    public class Rule_BrepToSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Rule_BrepToSurface class.
        /// </summary>
        public Rule_BrepToSurface()
          : base("Rule_BrepToSurface", "Nickname",
              "Exploding brep into six surfaces",
              "SimpleGrammar", "Kristiane")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Input geometry is a Brep", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surfaces", "lSrf", "List of Surfaces", GH_ParamAccess.list);
            pManager.AddTextParameter("Surface Name", "SrfName", "List with name of Surfaces", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // inputs
            Brep geo = new Brep();
            DA.GetData(0, ref geo);

            //code
            // transform a brep into six surfaces
            var faceLst = geo.Faces;
            List<Surface> srfLst = new List<Surface>();
            foreach (BrepFace face in faceLst)
            {
                srfLst.Add(face.ToNurbsSurface());
            }
       

            //vector of each surface
            List<Vector3d> vecs = new List<Vector3d>();
            foreach (Surface s in srfLst)
            { 
                Vector3d vec = s.NormalAt(0.5,0.5);
                vecs.Add(vec);
            }

            //name each surface
            List<string> srfName = new List<string>();
            foreach (Vector3d v in vecs)
                {
                if (v.Z > 0)
                {
                    srfName.Add("Top");
                }
                else if (v.Z < 0)
                {
                    srfName.Add("Bottom");
                }
                else if ((v.Z == 0 && v.X < 0) || (v.Z == 0 && v.X > 0))
                {
                    srfName.Add("Wall 1");
                }
                else if ((v.Z == 0 && v.Y < 0) || (v.Z == 0 && v.Y > 0))
                {
                    srfName.Add("Wall 2");
                }

            }

            //output
            DA.SetDataList(0,srfLst);
            DA.SetDataList(1, srfName);
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
            get { return new Guid("99C912F7-8E58-4603-890A-05910BC4AD5E"); }
        }
    }
}