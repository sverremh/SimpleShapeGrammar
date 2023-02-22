using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimpleShapeGrammar.Classes;
using SimpleShapeGrammar.Classes.Elements;

namespace SimpleShapeGrammar.Classes.Rules
{
    [Serializable]
    public class SH_AutoRule_01 : SH_Rule
    {
        // --- properties ---

        // from parent class
        // public State RuleState;
        // public string Name;

        public int EID { get; set; }
        public double T { get; set; }
        private double[] bounds = { 0.2, 0.8 };

        // --- constructors --- 

        public SH_AutoRule_01()
        {
            RuleState = State.alpha;
            Name = "SH_AutoRule_01";
        }

        public SH_AutoRule_01(int _eid, double _t)
        {
            RuleState = State.alpha;
            Name = "SH_AutoRule_01";

            EID = _eid;
            T = _t;
            
        }

        // --- methods ---
        // methods of parent class
        public override void NewRuleParameters(Random random, SH_SimpleShape ss) { }
        public override SH_Rule CopyRule(SH_Rule rule) 
        {
            throw new NotImplementedException();
        }
        public override string RuleOperation(ref SH_SimpleShape _ss) 
        {
            SH_Element line = _ss.Elems.Where(e => e.ID == EID).First();

            return "auto rule 01 applied.";
        }
        public override State GetNextState() 
        {
            throw new NotImplementedException();
        }
    }
}
