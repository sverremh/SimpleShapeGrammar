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
    public class SH_SimpleShape
    {
        // --- properties ---
        public int nodeCount = 0;
        public int elementCount = 0;
        public int supCount = 0;
        public List<NurbsCurve> NurbsCurves { get; set; }

        /// <summary>
        /// Dictionary of possible elements. Use the keys "Line" for lines, "Surface" for surface, and "Solid" for Breps. 
        /// </summary>
        // public Dictionary<string, List<SH_Element>> Elements { get; set; } = new Dictionary<string, List<SH_Element>>();
        public List<SH_Element> Elems { get; set; } = new List<SH_Element>();

        public List<SH_Node> Nodes { get; set; }
        public List<SH_Support> Supports { get; set; }
        public List<SH_LineLoad> LineLoads { get; set; }
        public List<SH_PointLoad> PointLoads { get; set; }
        public State SimpleShapeState { get; set; }

        // --- constructors ---
        public SH_SimpleShape()
        {
            // empty constructor
            
        }

        // --- methods ---
        public void AddLine(SH_Element _line)
        {
            Elems.Add(_line);
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
            return Elems.Select(e => (e as SH_Elem1D).Ln).ToList(); 
        }

        public void TranslateNode(Vector3d vec, int nodeInd)
        {
            
            SH_Node node = Nodes[nodeInd];
            Point3d newPoint = node.Pt + vec;
            // move the point
            Nodes[nodeInd].Pt = newPoint;
            // find the correct support in the list
            int supInd = Supports.IndexOf( Supports.Find(sup => sup.nodeInd == nodeInd) );
            // move the support position if present
            if(supInd != -1)
            {
                Supports[supInd].Position = newPoint;
            }
            

        }

        public SH_SimpleShape DeepCopy()
        {
            SH_SimpleShape simpleShapeCopy = new SH_SimpleShape();
            simpleShapeCopy.nodeCount = this.nodeCount;
            simpleShapeCopy.elementCount = this.elementCount;
            simpleShapeCopy.supCount = this.supCount;

            simpleShapeCopy.Elems = this.Elems;
            simpleShapeCopy.Nodes = this.Nodes;
            simpleShapeCopy.Supports = this.Supports;
            simpleShapeCopy.LineLoads = this.LineLoads;
            simpleShapeCopy.PointLoads = this.PointLoads;
            simpleShapeCopy.SimpleShapeState = this.SimpleShapeState;

            return simpleShapeCopy;
        }
        

    }
}
