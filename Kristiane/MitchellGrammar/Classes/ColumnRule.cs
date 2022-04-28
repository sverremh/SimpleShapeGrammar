using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleShapeGrammar.Classes;
using Rhino.Geometry;
using SimpleShapeGrammar.Classes.Elements;


namespace SimpleShapeGrammar.Kristiane.MitchellGrammar
{
    [Serializable]
    public class ColumnRule : SH_Rule
    {
        // --- properties ---

        // --- constructors ---
        public ColumnRule()
        {
            Name = "BrepSurfaceClass";
            RuleState = State.zeta;
        }

        // --- methods ---

        public override SH_Rule CopyRule(SH_Rule rule)
        {
            throw new NotImplementedException();
        }

        public override void NewRuleParameters(Random random, SH_SimpleShape ss)
        {
            throw new NotImplementedException();
        }

        public override string RuleOperation(SH_SimpleShape _ss)
        {
            // check if the state maches the simple shape state
            if (_ss.SimpleShapeState != RuleState)
            {
                return "The State is not compatible with Column.";
            }




            // change the state
            _ss.SimpleShapeState = State.eta;
            return "Column successfully applied.";
        }

        public override State GetNextState()
        {
            return State.eta;
        }

    }
}