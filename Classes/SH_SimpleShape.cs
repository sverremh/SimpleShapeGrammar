using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using SimpleShapeGrammar.Classes.Elements;

namespace SimpleShapeGrammar.Classes
{
    public enum State { alpha, beta, gamma, delta, epsilon, zeta, eta, theta, end }; // add more if needed. 
    [Serializable]
    public class SH_SimpleShape
    {
        // --- properties ---
        public int nodeCount = 0;
        public int elementCount = 0;
        public int supCount = 0;
        public string name = "";
        public List<NurbsCurve> NurbsCurves { get; set; }

        /// <summary>
        /// Dictionary of possible elements. Use the keys "Line" for lines, "Surface" for surface, and "Solid" for Breps. 
        /// </summary>
        public Dictionary<string, List<SH_Element>> Elements { get; set; } = new Dictionary<string, List<SH_Element>>();
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
            Elements["Line"].Add(_line);
        }
        public List<Line> GetLinesFromShape()
        {
            //Variable
            List<Line> lines = new List<Line>();

            //Get Line from each element
            var elLines = Elements["Line"];
            foreach (SH_Line sh_line in elLines)
            {
                // Create Start point
                Point3d sPt = sh_line.Nodes[0].Position;
                // Create End point
                Point3d ePt = sh_line.Nodes[1].Position;

                lines.Add(new Line(sPt, ePt));

            }

            return lines;
        }


        public List<Surface> GetSurfacesFromShape()
        {
            //Variable
            List<Surface> surfaces = new List<Surface>();

            //Get Surface from each element
            var elSurfaces = Elements["Surface"];
            foreach (SH_Surface sh_surface in elSurfaces)
            {
                surfaces.Add(sh_surface.elementSurface);
            }

            return surfaces;
        }

        public List<Point3d> GetSupportsFromShape()
        {
            //Variable
            List<Point3d> supports = new List<Point3d>();

            //Get Line from each element
            var elNodes = Nodes;
            foreach (SH_Node sh_node in elNodes)
            {
                if (sh_node.Position.Z == 0)
                {
                    supports.Add(sh_node.Position);
                }
            }

            return supports;
        }

        public void TranslateNode(Vector3d vec, int nodeInd)
        {

            SH_Node node = Nodes[nodeInd];
            Point3d newPoint = node.Position + vec;
            // move the point
            Nodes[nodeInd].Position = newPoint;
            // find the correct support in the list
            int supInd = Supports.IndexOf(Supports.Find(sup => sup.nodeInd == nodeInd));
            // move the support position if present
            if (supInd != -1)
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

            simpleShapeCopy.Elements = this.Elements;
            simpleShapeCopy.Nodes = this.Nodes;
            simpleShapeCopy.Supports = this.Supports;
            simpleShapeCopy.LineLoads = this.LineLoads;
            simpleShapeCopy.PointLoads = this.PointLoads;
            simpleShapeCopy.SimpleShapeState = this.SimpleShapeState;

            return simpleShapeCopy;
        }


    }
}
