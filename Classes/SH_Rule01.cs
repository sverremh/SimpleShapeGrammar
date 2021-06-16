using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;


namespace SimpleShapeGrammar.Classes
{
    [Serializable]
    public class SH_Rule01 : SH_Rule
    {
        // --- properties ---
        public double P0X { get; private set; }
        public double P0Y { get; private set; }
        public double P0Z { get; private set; }
        public double P1X { get; private set; }
        public double P1Y { get; private set; }

        public double P1Z { get; private set; }

        public State RuleState = State.alpha;

        // --- constructors ---
        public SH_Rule01()
        {
            // empty constructor
        }
        public SH_Rule01(Vector3d _translate_start, Vector3d _translate_end)
        {
            P0X = _translate_start.X; P0Y = _translate_start.Y; P0Z = _translate_start.Z;
            P1X = _translate_end.X; P1Y = _translate_end.Y; P1Z = _translate_end.Z;
        }

        // --- methods ---
        public override void RuleOperation(SH_SimpleShape _ss)
        {
            // check if the state maches the simple shape state
            if (_ss.SimpleShapeState != RuleState)
            {
                return;
            }

            // take the 1st element
            SH_Line sh_line = _ss.Lines[0];

            // apply the change
            sh_line.Nodes[0].X += P0X;
            sh_line.Nodes[0].Y += P0Y;
            sh_line.Nodes[0].Z += P0Z;
            sh_line.Nodes[1].X += P1X;
            sh_line.Nodes[1].Y += P1Y;
            sh_line.Nodes[1].Z += P1Z;

            // change the state
            _ss.SimpleShapeState = State.beta;

        }

    }
}
