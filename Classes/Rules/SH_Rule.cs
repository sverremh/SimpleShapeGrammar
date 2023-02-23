using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGrammar.Classes.Rules
{
    [Serializable]
    public abstract class SH_Rule : ISH_Rule
    {
        public State RuleState;
        public string Name;

        public SH_Rule()
        { 
            
        }

        public abstract void NewRuleParameters(Random random, SG_Shape ss);
        public abstract SH_Rule CopyRule(SH_Rule rule);

        public virtual string RuleOperation(ref SG_Shape _ss) { return ""; }
        public virtual string RuleOperation(ref SG_Shape _ss, ref SG_Genotype _st) { return ""; }

        public abstract State GetNextState();

        // for child class
        //public override void NewRuleParameters(Random random, SG_Shape ss) { }
        //public override SH_Rule CopyRule(SH_Rule rule)
        //{
        //    throw new NotImplementedException();
        //}
        //public override string RuleOperation(SG_Shape _ss)
        //{
        //    throw new NotImplementedException();
        //}
        //public override State GetNextState()
        //{
        //    throw new NotImplementedException();
        //}
    }


}
