using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
namespace SimpleShapeGrammar.Classes
{
    public enum State { alpha = 0, beta = 1, gamma = 2 };
    [Serializable]
    public class SH_SimpleShape
    {
        // --- properties ---
        public int NodeCount = 0;
        public int LineCount = 0;
        public List<SH_Element> Lines { get; set; } = new List<SH_Element>();
        public List<SH_Node> Nodes { get; set; }
        public State SimpleShapeState { get; set; }

        // --- constructors ---
        public SH_SimpleShape()
        {
            // empty constructor
        }

        // --- methods ---
        public void AddLine(SH_Element _line)
        {
            Lines.Add(_line);
        }
        public List<Line> GetLinesFromShape()
        {
            //Variable
            List<Line> lines = new List<Line>();

            //Get Line from each element
            foreach (SH_Element sh_line in Lines)
            {
                // Create Start point
                Point3d sPt = sh_line.Nodes[0].Position;
                // Create End point
                Point3d ePt = sh_line.Nodes[1].Position;             

                lines.Add(new Line(sPt, ePt));

            }

            return lines; 
        }
    }
}
