using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;


namespace ShapeGrammar.Classes.Elements
{
    [Serializable]
    public class SH_Elem1D : SH_Element
    {
        // -- properties

        // Properties from SH_Element class
        // public int? ID { get; set; }
        // public string elementName { get; set; }
        // public SH_Node[] Nodes { get; set; }

        public Line Ln { get; set; }

        public SH_CrossSection_Beam CrossSection { get; set; }


        // --- constructors ---
        public SH_Elem1D()
        {

        }
        public SH_Elem1D(SH_Node[] _nodes, int? _id)
        {

            ID = _id;
            Nodes = _nodes;
            CreateLine();
        }
        public SH_Elem1D(SH_Node[] _nodes, int? _id, string _el_name)
        {
            ID = _id;
            Nodes = _nodes;
            elementName = _el_name;
            CreateLine();
        }

        public SH_Elem1D(Line _ln, int? _id, string _el_name, SH_CrossSection_Beam _cs)
        {
            ID = _id;
            elementName = _el_name;
            Ln = _ln;

            SH_Node[] nodes = new SH_Node[2];
            nodes[0] = new SH_Node(Ln.From, null);
            nodes[1] = new SH_Node(Ln.To, null);

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
