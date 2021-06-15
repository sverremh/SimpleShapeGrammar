using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShapeGrammar.Classes
{
    class SH_Node
    {
        // --- properties ---

        public int ID { get; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public SH_Support support { get; set; }



        // --- constructors --- 
        public SH_Node()
        {
            SH_UtilityClass.NodeCount += 1;
            ID = SH_UtilityClass.NodeCount;
            
        }
        public SH_Node(double _x, double _y, double _z)
        {
            SH_UtilityClass.NodeCount += 1;
            ID = SH_UtilityClass.NodeCount;
            X = _x;
            Y = _y;
            Z = _z;

        }

        // --- methods ---
        


    }
}
