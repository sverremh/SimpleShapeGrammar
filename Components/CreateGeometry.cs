﻿using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using SimpleShapeGrammar.Classes;
namespace SimpleShapeGrammar.Components
{
    public class CreateGeometry : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateGeometry class.
        /// </summary>
        public CreateGeometry()
          : base("CreateGeometry", "geom",
              "Create geometry from Simple Shape",
              "SimpleGrammar", "Utility")
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
            pManager.AddLineParameter("Lines", "l", "Lines from the simple shape", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("Surfaces", "srf", "Surfaces from the simple shape", GH_ParamAccess.list);
            pManager.AddPointParameter("Supports", "sup", "Supports from the simple shape", GH_ParamAccess.list);
            pManager.AddGenericParameter("Test", "test", "test", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- variables ---
            SH_SimpleShape simpleShape = new SH_SimpleShape();

            // --- input --- 
            if (!DA.GetData(0, ref simpleShape)) return;

            // --- solve ---
            List<Line> lines = simpleShape.GetLinesFromShape();
            List<Surface> surfaces = simpleShape.GetSurfacesFromShape();
            List<Point3d> supports = simpleShape.GetSupportsFromShape();

            List<int?> ids = new List<int?>();
            foreach (var item in simpleShape.Elements["Line"])
            {
                ids.Add(item.ID);
            }


            // --- output ---
            DA.SetDataList(0, lines);
            DA.SetDataList(1, surfaces);
            DA.SetDataList(2, supports);
            DA.SetDataList(3, ids);
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
                return SimpleShapeGrammar.Properties.Resources.icons_I_DrawCS;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("70361ee6-5a32-45c9-a4c7-8e7228a2c48d"); }
        }
    }
}