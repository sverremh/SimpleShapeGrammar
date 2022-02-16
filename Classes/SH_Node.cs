using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using SimpleShapeGrammar.Classes.Elements;
namespace SimpleShapeGrammar.Classes
{
    [Serializable]
    public class SH_Node
    {           

        // --- properties ---
        public int? ID { get; set;}
        public Point3d Position { get; set; }
        public SH_Support Support { get; set; } // Do the node need this information? It does not in Karamba3D
        public List<SH_Element> Elements { get; set; }

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
        /*
        public void Translate(Vector3d vector)
        {
            Position = Position + vector; // translate the point

            // update the corresponding node. 
            Support.Position += vector;


        }*/


    }
}
