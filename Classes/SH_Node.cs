using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
namespace SimpleShapeGrammar.Classes
{
    [Serializable]
    public class SH_Node
    {           

        // --- properties ---

        public int ID { get; }
        public Point3d Position { get; set; }

        public SH_Support Support { get; set; }



        // --- constructors --- 
        public SH_Node()
        {
            SH_UtilityClass.NodeCount += 1;
            ID = SH_UtilityClass.NodeCount;
            
        }
        public SH_Node(Point3d _location)
        {
            SH_UtilityClass.NodeCount += 1;
            ID = SH_UtilityClass.NodeCount;
            
            Support = new SH_Support("000000", Position);
            Position = _location;
        }

        // --- methods ---
        


    }
}
