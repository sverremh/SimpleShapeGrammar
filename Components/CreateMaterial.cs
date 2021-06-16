using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;

namespace SimpleShapeGrammar.Components
{
    public class CreateMaterial : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateMaterial class.
        /// </summary>
        public CreateMaterial()
          : base("CreateMaterial", "material",
              "CreateMaterial",
              "SimpleGrammar", "Material")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "name", "Name.", GH_ParamAccess.item, "S355");
            pManager.AddNumberParameter("Young's Modulus", "E", "Young's Modulus of material [N/mm^2]", GH_ParamAccess.item, 210000);
            pManager.AddNumberParameter("Poisson's Ratio", "v", "Poisson's ratio of the material", GH_ParamAccess.item, 0.3);
            pManager.AddNumberParameter("Yield Strength", "fy", "Yield strength [N/mm^2]", GH_ParamAccess.item, 355);
            pManager.AddNumberParameter("Density", "rho", "Density of material [kg/m^3]", GH_ParamAccess.item, 7800);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "m", "SH_Material Class", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            double e = 0.0;
            double v = 0.0;
            double fy = 0.0;
            string name = "";
            double rho = 0.0;

            // --- input ---
            DA.GetData(0, ref name);
            DA.GetData(1, ref e);
            DA.GetData(2, ref v);
            DA.GetData(3, ref fy);
            DA.GetData(4, ref rho);

            // --- solve ---
            SH_Material material = new SH_Material(name, e, v, fy, rho);

            // --- output ---
            DA.SetData(0, material);
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
            get { return new Guid("465674f0-3773-4201-b55d-a6010b4368a5"); }
        }
    }
}