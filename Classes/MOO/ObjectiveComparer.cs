using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGrammar.Classes
{
    class ObjectiveComparer : IComparer<List<double>>
    {
        // public int NumVars = 0;
        public int Compare(List<double> x, List<double> y)
        {
            if (x[0] >= y[0])
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
