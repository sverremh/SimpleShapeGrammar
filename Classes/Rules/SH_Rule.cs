using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShapeGrammar.Classes
{
    [Serializable]
    public abstract class SH_Rule : ISH_Rule
    {
        public State RuleState;
        public string Name;

        public SH_Rule()
        {

        }

        public abstract void NewRuleParameters(Random random, SH_SimpleShape ss);
        public abstract SH_Rule CopyRule(SH_Rule rule);

        public abstract string RuleOperation(SH_SimpleShape _ss);

        public abstract State GetNextState();
    }


}
