using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimpleShapeGrammar.Classes;

namespace SimpleShapeGrammar.Classes.Rules
{
    [Serializable]
    public class SH_AutoRule_01 : SH_Rule
    {
        // --- properties ---

        // --- constructors ---

        // --- methods ---

        public override void NewRuleParameters(Random random, SH_SimpleShape ss) { }
        public override SH_Rule CopyRule(SH_Rule rule) 
        {
            throw new NotImplementedException();
        }
        public override string RuleOperation(SH_SimpleShape _ss) 
        {
            throw new NotImplementedException();
        }
        public override State GetNextState() 
        {
            throw new NotImplementedException();
        }
    }
}
