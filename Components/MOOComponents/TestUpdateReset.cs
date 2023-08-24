using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Rhino.Geometry;

namespace ShapeGrammar.Components.MOOComponents
{
    public class TestUpdateReset : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the TestUpdateReset class.
        /// </summary>
        public TestUpdateReset()
          : base("TestUpdateReset", "Nickname",
              "Description",
              "SimpleGrammar", "MOO")
        {
        }

        // -- properties -- 
        public double? outVal = 0.0;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "t", "True if component should run", GH_ParamAccess.item); // 0
            pManager.AddBooleanParameter("Reset", "res", "Button", GH_ParamAccess.item); // 1
            pManager.AddNumberParameter("Output", "num", "Number to output", GH_ParamAccess.item); // 2
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Output", "out", "test output", GH_ParamAccess.item); // 0
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // -- input --
            bool run = false; // 0
            bool reset = false; // 1
            double output = 0.0; // 2

            // -- register input -- 
            if (!DA.GetData(0, ref run)) return;
            if (!DA.GetData(1, ref reset)) return; 
            if(!DA.GetData(2, ref output)) return;


            // -- solve the problem --

            // access the Active GH Document
            var doc = OnPingDocument();
            if (doc == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Something is wrong when calling the document.");
                return;
            }


            double? outputData = null;
            // double testOut = 0.0;
            if (!(this.Params.Input[0].Sources[0] is GH_BooleanToggle runBool))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input component \"Run\" should be a Boolean Toggle.");
                return;
            }

            if (run)
            {
                /*
                if (notRunned)
                {
                    outputData = output;
                    notRunned = false;
                }*/
                outputData = output;
                //outputData = output; // declare the output
                runBool.Value = false;

                var callbackDelegate = new GH_Document.GH_ScheduleDelegate(Callback);

                doc.ScheduleSolution(5, callbackDelegate);
                outVal = outputData;
            }
            else AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Set the Boolean to \"True\" to run the component"); 

            if (reset)
            {
                runBool.Value = false;
                outVal = null; 
                runBool.ExpireSolution(true); // This expires the input toggle component. Then the downstream component is also recomputed.  
                


            }

            

            
             


            
            // -- output -- 
            DA.SetData(0, outVal); // 0
            
        }

        private void Callback(GH_Document doc)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Something has changed");
            var runBool = this.Params.Input[0].Sources[0] as GH_BooleanToggle;
            runBool.ExpireSolution(true);
            //this.Params.Input[2].RemoveAllSources();

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
            get { return new Guid("40B19AE5-88B0-4457-AE80-ABA0CA2A76CD"); }
        }
    }
}