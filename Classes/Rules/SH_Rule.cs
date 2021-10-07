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
        public SH_Rule()
        { 
            
        }

        public abstract void NewRuleParameters(Random random, int numLines);

        public abstract string RuleOperation(SH_SimpleShape _ss);

        public abstract State GetNextState();
    }


}
