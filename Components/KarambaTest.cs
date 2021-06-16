using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Karamba.Models;
using Karamba.GHopper.Models;
namespace SimpleShapeGrammar.Components
{
    public class KarambaTest : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the KarambaTest class.
        /// </summary>
        public KarambaTest()
          : base("KarambaTest", "Nickname",
              "Description",
              "SimpleGrammar", "Karamba")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Model(), "Model_in", "Model_in",
                       "Model to be manipulated", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Model(), "Model_out", "Model_out",
                        "Model after eliminating all tension elements");
            pManager.Register_BooleanParam("isActive", "isActive",
                "List of boolean values corresponding to each element in the model." +
                "True if the element is active.");            
            pManager.Register_DoubleParam("maximum displacement", "maxDisp",
                " Maximum displacement [m] of the model after eliminationprocess.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Model in_gh_model = null;
            if (!DA.GetData<GH_Model>(0, ref in_gh_model)) return;
            var model = in_gh_model.Value;

            // maximum number of iterations
            int max_iter = 10;
            DA.GetData<int>(1, ref max_iter);

            // load case to consider for elimination of elements
            int lc_num = 0;

            // clone model to avoid side effects
            model = (Karamba.Models.Model)model.Clone();

            // clone its elements to avoid side effects
            model.cloneElements();

            // clone the feb-model to avoid side effects
            model.deepCloneFEModel();

            string singular_system_msg = "The stiffness matrix of the system is singular.";

            // do the iteration and remove elements with tensile axial forces
            for (int iter = 0; iter < max_iter; iter++)
            {
                // create an analysis and response object for calculating and retrieving results
                feb.Deform analysis = new feb.Deform(model.febmodel);
                feb.Response response = new feb.Response(analysis);

                try
                {
                    // calculate the displacements
                    response.updateNodalDisplacements();
                    // calculate the member forces
                    response.updateMemberForces();
                }
                catch
                {
                    // send an error message in case something went wrong
                    throw new Exception(singular_system_msg);
                }

                // check the normal force of each element and deactivate those under tension
                double N, V, M;
                bool has_changed = false;
                foreach (var elem in model.elems)
                {
                    // retrieve resultant cross section forces
                    elem.resultantCroSecForces(model, lc_num, out N, out V, out M);
                    // check whether normal force is tensile
                    if (!(N >= 0)) continue;
                    // set element inactive
                    elem.set_is_active(model, false);
                    has_changed = true;
                }

                // leave iteration loop if nothing changed
                if (!has_changed) break;

                // if something changed inform the feb-model about it (otherwise it won't recalculate)
                model.febmodel.touch();

                // this guards the objects from being freed prematurely
                GC.KeepAlive(analysis);
                GC.KeepAlive(response);
            }

            // update model to its final state
            double max_disp = 0;
            try
            {
                // create an analysis and response object for calculating and retrieving results
                feb.Deform analysis = new feb.Deform(model.febmodel);
                feb.Response response = new feb.Response(analysis);

                // calculate the displacements
                response.updateNodalDisplacements();
                // calculate the member forces
                response.updateMemberForces();

                max_disp = response.maxDisplacement();

                // this guards the objects from being freed prematurely
                GC.KeepAlive(analysis);
                GC.KeepAlive(response);
            }
            catch
            {
                // send an error message in case something went wrong
                throw new Exception(singular_system_msg);
            }

            // set up list of true/false values that corresponds to the element states
            List<bool> elem_activity = new List<bool>();
            foreach (var elem in model.elems)
            {
                elem_activity.Add(elem.is_active);
            }

            DA.SetData(0, new GH_Model(model));
            DA.SetDataList(1, elem_activity);
            DA.SetData(2, max_disp);
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