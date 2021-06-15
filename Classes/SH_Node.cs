using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShapeGrammar.Classes
{
    public class SH_Node
    {           
        // --- properties ---
        private int ID { get; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        private SH_Support support { get; set; }

        // --- constructors --- 
        public SH_Node(int _id)
        {
            ID = _id;
        }
        public SH_Node(int _id, double _x, double _y, double _z)
        {
            ID = _id;
            X = _x;
            Y = _y;
            Z = _z;

        }

        // --- methods ---
        


    }
}
