using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;


namespace SimpleShapeGrammar.Classes.Elements
{
    [Serializable]
    public class SH_Line : SH_Element
    {
        // -- properties

        public NurbsCurve NurbsCurve { get; set; }

        // Maybe we need an abstract element class with line elements and surface elements as inheriting classes?
        //public List<Surface> Surfaces { get; set; } 


        public SH_CrossSection_Beam CrossSection { get; set; }


        // --- constructors ---
        public SH_Line()
        {

        }
        public SH_Line(SH_Node[] _nodes, int? _id)
        {

            ID = _id;
            Nodes = _nodes;
            CreateNurbs();
        }
        public SH_Line(SH_Node[] _nodes, int? _id, string _el_name)
        {
            ID = _id;
            Nodes = _nodes;
            elementName = _el_name;
            CreateNurbs();
        }

        /// <summary>
        /// Create a SH_Line using the existing nodes of a SH_SimpleShape instance
        /// </summary>
        /// <param name="_ss">The existing shape</param>
        /// <param name="line">A Rhino line</param>
        /// <param name="_name">The name of the element</param>
        public SH_Line(SH_SimpleShape _ss, Line rhinoLine, string _name)
        {
            List<SH_Node> nodes = _ss.Nodes;

            // find the closest nodes to the start- and end point of the line. 
            Point3d sPt = rhinoLine.From;
            Point3d ePt = rhinoLine.To;
            double tol = 0.001; // tolerance
            try
            {
                SH_Node startNode = nodes.First(n => n.Position.DistanceToSquared(sPt) < tol);
                SH_Node endNode = nodes.First(n => n.Position.DistanceToSquared(ePt) < tol);
                int id = _ss.elementCount;
                ID = id;
                Nodes = new []{startNode, endNode};
                elementName = _name;
                CreateNurbs();
                _ss.elementCount++;


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        // --- methods ---
        private void CreateNurbs()
        {
            NurbsCurve = NurbsCurve.Create(false, 1, new Point3d[] { Nodes[0].Position, Nodes[1].Position });
        }
    }
}
