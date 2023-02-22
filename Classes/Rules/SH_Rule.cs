using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShapeGrammar.Classes.Rules
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

        public abstract string RuleOperation(ref SH_SimpleShape _ss);
        
        public abstract State GetNextState();

        // for child class
        //public override void NewRuleParameters(Random random, SH_SimpleShape ss) { }
        //public override SH_Rule CopyRule(SH_Rule rule)
        //{
        //    throw new NotImplementedException();
        //}
        //public override string RuleOperation(SH_SimpleShape _ss)
        //{
        //    throw new NotImplementedException();
        //}
        //public override State GetNextState()
        //{
        //    throw new NotImplementedException();
        //}
    }


}
