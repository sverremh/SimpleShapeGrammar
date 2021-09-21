using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShapeGrammar.Classes
{
    class ObjectiveComparer : IComparer<List<double>>
    {
        public int NumVars;
        public int Compare(List<double> x, List<double> y)
        {
            if (x[NumVars] >= y[NumVars])
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
