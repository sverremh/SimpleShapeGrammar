using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SimpleShapeGrammar.Classes
{
    public class SH_Rule02 : SH_Rule
    {
        // --- properties ---
        public State RuleState = State.beta;
        public int LineID { get; set; }
        public double Param { get; set; }

        // --- constructors ---
        public SH_Rule02()
        {
            
        }

        public SH_Rule02(int _lineid, double _param)
        {
            LineID = _lineid;
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
            SH_Line line = _ss.Lines.Where(l => l.ID == LineID).First();

            // add the intermediate node
            SH_Node newNode = AddNode(line, Param);

            // create 2x lines 
            List<SH_Node> nodes = new List<SH_Node>();
            SH_Line newLine0 = new SH_Line(new SH_Node[] { line.Nodes[0], newNode });
            SH_Line newLine1 = new SH_Line(new SH_Node[] { newNode, line.Nodes[1] });

            // add the lines to the _ss list
            _ss.Lines.Add(newLine0);
            _ss.Lines.Add(newLine1);

            // remove the line from the line list
            _ss.Lines.RemoveAll((l) => l.ID == LineID);


            // no change in the state (remains in beta state)
            // _ss.SimpleShapeState = State.beta; 

        }

        private SH_Node AddNode(SH_Line _line, double _t)
        {
            double sx = _line.Nodes[0].X;
            double sy = _line.Nodes[0].Y;
            double sz = _line.Nodes[0].Z;
            double ex = _line.Nodes[1].X;
            double ey = _line.Nodes[1].Y;
            double ez = _line.Nodes[1].Z;

            double mx = (1 - _t) * sx + _t * ex;
            double my = (1 - _t) * sy + _t * ey;
            double mz = (1 - _t) * sz + _t * ez;

            SH_Node newNode = new SH_Node(mx, my, mz);

            return newNode;
        }


    }
}
