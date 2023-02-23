using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using ShapeGrammar.Classes.Elements;
namespace ShapeGrammar.Classes
{
    [Serializable] 
    public class SG_Node
    {           

        // --- properties ---
        public int ID { get; set;}
        public Point3d Pt { get; set; }
        public SG_Support Support { get; set; }
        public List<SG_Element> Elements { get; set; } = new List<SG_Element>();

        // --- constructors --- 
        public SG_Node()
        {
        }
        public SG_Node(Point3d _location, int _id)
        {
            ID = _id;
            Pt = _location;

            Support = new SG_Support("000000", Pt);
            Support.Node = this;
            
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
