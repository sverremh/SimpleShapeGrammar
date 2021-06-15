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

        // --- constructors ---
        public SH_Rule02()
        {

        }

        // --- methods ---
        void RuleOperation(SH_SimpleShape _ss, int _lineid, double _param)
        {
            // check if the state maches the simple shape state
            if (_ss.SimpleShapeState != RuleState)
            {
                return;
            }

            // choose the line to split
            SH_Line line = _ss.Lines.Where(l => l.ID == _lineid).First();

            // add the intermediate node
            SH_Node newNode = AddNode(line, _param);

            // create 2x lines 
            List<SH_Node> nodes = new List<SH_Node>();
            SH_Line newLine0 = new SH_Line(new SH_Node[] { line.nodes[0], newNode });
            SH_Line newLine1 = new SH_Line(new SH_Node[] { newNode, line.nodes[1] });

            // add the lines to the _ss list
            _ss.Lines.Add(newLine0);
            _ss.Lines.Add(newLine1);

            // remove the line from the line list
            _ss.Lines.RemoveAll((l) => l.ID == _lineid);


            // change the state
            // _ss.SimpleShapeState = State.beta;

        }

        private SH_Node AddNode(SH_Line _line, double _t)
        {
            double sx = _line.nodes[0].X;
            double sy = _line.nodes[0].Y;
            double sz = _line.nodes[0].Z;
            double ex = _line.nodes[1].X;
            double ey = _line.nodes[1].Y;
            double ez = _line.nodes[1].Z;

            double mx = (1 - _t) * sx + _t * ex;
            double my = (1 - _t) * sy + _t * ey;
            double mz = (1 - _t) * sz + _t * ez;

            SH_Node newNode = new SH_Node(mx, my, mz);

            return newNode;
        }

        public SH_Rule02()
        {

        }


        public override void RuleOperation(SH_SimpleShape _ss)
        {
            throw new NotImplementedException();
        }

    }
}
