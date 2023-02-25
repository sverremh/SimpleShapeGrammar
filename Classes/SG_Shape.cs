using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using ShapeGrammar.Classes.Elements;

namespace ShapeGrammar.Classes
{
    public enum State { alpha, beta, gamma, delta, epsilon, zeta, eta, theta, end}; // add more if needed. 

    [Serializable]
    public class SG_Shape
    {
        // --- properties ---
        public int nodeCount = 0;
        public int elementCount = 0;
        public List<NurbsCurve> NurbsCurves { get; set; }

        public List<SG_Element> Elems { get; set; } = new List<SG_Element>();

        public List<SG_Node> Nodes { get; set; }
        public List<SG_Support> Supports { get; set; }
        public List<SH_LineLoad> LineLoads { get; set; }
        public List<SH_PointLoad> PointLoads { get; set; }
        public State SimpleShapeState { get; set; }

        // --- constructors ---
        public SG_Shape()
        {
            // empty constructor
            
        }

        // --- methods ---
        public void AddLine(SG_Element _line)
        {
            Elems.Add(_line);
        }

        public void AddNewElement(SG_Elem1D _e)
        {
            // element counter
            _e.ID = elementCount;
            elementCount++;
            Elems.Add(_e);

            // search existent nodes
            foreach (SG_Node nd in _e.Nodes)
            {
                SG_Node targetNode;

                // test if there is already a node in this position
                if (Nodes.Any(n => n.Pt.DistanceToSquared(nd.Pt) < 0.001))
                {
                    targetNode = Nodes.Where(n => n.Pt.DistanceToSquared(nd.Pt) < 0.001).First();

                    targetNode.Elements.Add(_e);
                    continue;
                }

                // in case it is a new node
                nd.ID = nodeCount;
                nd.Elements.Add(_e);
                Nodes.Add(nd);
                nodeCount++;
            }

        }

        //public void AddSurface(SH_Element _surface)
        //{
        //    if (!Elements.ContainsKey("Surface"))
        //    {
        //        Elements["Surface"] = new List<SH_Element>();
        //    }
            
        //    Elements["Surface"].Add(_surface);
        //}

        public List<Line> GetLinesFromShape()
        {
            return Elems.Select(e => (e as SG_Elem1D).Ln).ToList(); 
        }

        public void TranslateNode(Vector3d vec, int nodeInd)
        {
            SG_Node node = Nodes[nodeInd];
            Point3d newPoint = node.Pt + vec;
            // move the point
            Nodes[nodeInd].Pt = newPoint;
        }

        public SG_Shape DeepCopy()
        {
            SG_Shape simpleShapeCopy = new SG_Shape
            {
                nodeCount = this.nodeCount,
                elementCount = this.elementCount,

                Elems = this.Elems,
                Nodes = this.Nodes,
                Supports = this.Supports,
                LineLoads = this.LineLoads,
                PointLoads = this.PointLoads,
                SimpleShapeState = this.SimpleShapeState
            };

            return simpleShapeCopy;
        }
        

    }
}
