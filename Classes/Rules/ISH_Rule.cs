using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGrammar.Classes.Rules
{
    public interface ISH_Rule
    {
        void NewRuleParameters(Random random, SG_Shape simpleShape);
        SH_Rule CopyRule(SH_Rule rule);

        

    }
}
