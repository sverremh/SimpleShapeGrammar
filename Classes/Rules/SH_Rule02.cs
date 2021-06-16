using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace SimpleShapeGrammar.Classes
{
    [Serializable]
    public class SH_Rule02 : SH_Rule
    {
        // --- properties ---
        public State RuleState = State.beta;
        public int LineIndex { get; set; }
        public double Param { get; set; }

        // --- constructors ---
        public SH_Rule02()
        {
            
        }

        public SH_Rule02(int _line_index, double _param)
        {
            LineIndex = _line_index;
            Param = _param;
        }

        // --- methods ---
        public override void RuleOperation(SH_SimpleShape _ss)
        {
            // check if the state maches the simple shape state
            if (_ss.SimpleShapeState != RuleState)
            {
                return;
            }

            // choose the line to split
            //SH_Element line = _ss.Lines.Where(l => l.ID == LineID).First(); DELETE IF OK
            SH_Element line = _ss.Lines[LineIndex];


            int elInd = _ss.Lines.IndexOf(line);
            // add the intermediate node
            SH_Node newNode = AddNode(line, Param, _ss.NodeCount);
            _ss.NodeCount++;

            // create 2x lines 
            List<SH_Node> nodes = new List<SH_Node>();
           
            SH_Element newLine0 = new SH_Element(new SH_Node[] { line.Nodes[0], newNode }, _ss.LineCount);
            _ss.LineCount++;
            SH_Element newLine1 = new SH_Element(new SH_Node[] { newNode, line.Nodes[1] }, _ss.LineCount);
            _ss.LineCount++;

            
            
            // remove the line which has been split
            _ss.Lines.RemoveAt(elInd);
            _ss.Lines.Insert(elInd, newLine1);

            // insert the new lines in its position
            _ss.Lines.Insert(elInd, newLine0);

            // no change in the state (remains in beta state)
            // _ss.SimpleShapeState = State.beta; 

        }

        private SH_Node AddNode(SH_Element _line, double _t, int _id)
        {
            double mx = (1 - _t) * _line.Nodes[0].Position.X + _t * _line.Nodes[1].Position.X;
            double my = (1 - _t) * _line.Nodes[0].Position.Y + _t * _line.Nodes[1].Position.Y;
            double mz = (1 - _t) * _line.Nodes[0].Position.Z + _t * _line.Nodes[1].Position.Z;
            Point3d newPoint = new Point3d(mx, my, mz);
            SH_Node newNode = new SH_Node(newPoint, _id);

            return newNode;
        }


    }
}
