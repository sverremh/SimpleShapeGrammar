using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
namespace SimpleShapeGrammar.Classes
{
    public enum State { alpha = 0, beta = 1, gamma = 2 , end = 3};
    [Serializable]
    public class SH_SimpleShape
    {
        // --- properties ---
        public int nodeCount = 0;
        public int elementCount = 0;
        public int supCount = 0;
        public List<SH_Element> Elements { get; set; } //= new List<SH_Element>(); // Is the final initiation really necessary?
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
            Elements.Add(_line);
        }
        public List<Line> GetLinesFromShape()
        {
            //Variable
            List<Line> lines = new List<Line>();

            //Get Line from each element
            foreach (SH_Element sh_line in Elements)
            {
                // Create Start point
                Point3d sPt = sh_line.Nodes[0].Position;
                // Create End point
                Point3d ePt = sh_line.Nodes[1].Position;             

                lines.Add(new Line(sPt, ePt));

            }

            return lines; 
        }

        public void TranslateNode(Vector3d vec, int nodeInd)
        {
            
            SH_Node node = Nodes[nodeInd];
            Point3d newPoint = node.Position + vec;
            // move the point
            Nodes[nodeInd].Position = newPoint;
            // find the correct support in the list
            int supInd = Supports.IndexOf( Supports.Find(sup => sup.nodeInd == nodeInd) );
            // move the support position if present
            if(supInd != -1)
            {
                Supports[supInd].Position = newPoint;
            }
            

        }
    }
}
