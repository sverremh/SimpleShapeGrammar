using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel.Special;
using Grasshopper.Kernel;
using System.Windows.Forms;
using Rhino.Geometry;
using SimpleShapeGrammar.Components.MOOComponents;

namespace SimpleShapeGrammar.Classes
{
    class MOOComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        // -- properties --
        FirstGrammarMOO MyComponent;
        List<GH_NumberSlider> variablesSliders = new List<GH_NumberSlider>();
        public int solutionsCounter = 0; // count the number of designs evaluated
        (List<SH_Rule> genome, List<double> fitness) allSolutionsTrack;

        public MOOComponentAttributes(IGH_Component component) : base(component)
        {
            MyComponent = (FirstGrammarMOO)component;
        }
    }
}
