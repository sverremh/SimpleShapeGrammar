using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;


namespace ShapeGrammar.Classes.Elements
{
    [Serializable]
    public class SG_Elem1D : SG_Element
    {
        // -- properties

        // Properties from SH_Element class
        // public int? ID { get; set; }
        // public string elementName { get; set; }
        // public SH_Node[] Nodes { get; set; } 

        public Line Ln { get; set; }
        public SH_CrossSection_Beam CrossSection { get; set; }

        // --- constructors ---
        public SG_Elem1D()
        {

        }
        public SG_Elem1D(SG_Node[] _nodes, int _id)
        {

            ID = _id;
            Nodes = _nodes;
            CreateLine();
        }
        public SG_Elem1D(SG_Node[] _nodes, int _id, string _el_name)
        {
            ID = _id;
            Nodes = _nodes;
            Name = _el_name;
            
            
            CreateLine();


        }

        public SG_Elem1D(Line _ln, int _id, string _el_name, SH_CrossSection_Beam _cs)
        {
            ID = _id;
            Name = _el_name;
            Ln = _ln;
            CrossSection = _cs;

            SG_Node[] nodes = new SG_Node[2];
            nodes[0] = new SG_Node(Ln.From, -999);
            nodes[1] = new SG_Node(Ln.To, -999);

            Nodes = nodes;
        }


        // --- methods ---

        //private void CreateNurbs()
        //{
        //    NurbsCurve = NurbsCurve.Create(false, 1, new Point3d[] { Nodes[0].Position, Nodes[1].Position });
        //} 

        private void CreateLine()
        {
            Ln = new Line(Nodes[0].Pt, Nodes[1].Pt);
        }
    }
}
