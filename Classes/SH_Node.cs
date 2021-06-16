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


        public int? ID { get; set;}
        public Point3d Position { get; set; }


        public SH_Support Support { get; set; }



        // --- constructors --- 
        public SH_Node()
        {
            // empty            
        }
        public SH_Node(Point3d _location, int? _id)
        {
            ID = _id;
            
            Support = new SH_Support("000000", Position);
            Position = _location;
        }

        // --- methods ---
        


    }
}
