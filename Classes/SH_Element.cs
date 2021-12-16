using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
namespace SimpleShapeGrammar.Classes
{
    [Serializable] 
    public class SH_Element //: SH_CrossSection_Beam
    {
        // --- properties ---
        
        public int? ID { get; set; }
        public string elementName { get; set; }
        public SH_Node[] Nodes { get; }

        public NurbsCurve NurbsCurve { get; set; }

        public SH_CrossSection_Beam CrossSection { get; set; }
        

        // --- constructors ---
        public SH_Element()
        {

        }
        public SH_Element(SH_Node[] _nodes, int? _id) 
        {
            
            ID = _id;
            Nodes = _nodes;
            CreateNurbs();
        }
        public SH_Element(SH_Node[] _nodes, int? _id, string _el_name)
        {
            ID = _id;
            Nodes = _nodes;
            elementName = _el_name;
            CreateNurbs();
        }


        // --- methods ---
        private void CreateNurbs()
        {
            NurbsCurve = NurbsCurve.Create(false, 1, new Point3d[]{Nodes[0].Position, Nodes[1].Position});
        }
    }
}
