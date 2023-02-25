using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Rhino.Geometry;

using ShapeGrammar.Classes.Elements;

namespace ShapeGrammar.Classes.Rules
{
    [Serializable]
    public class SG_AutoRule04 : SG_Rule
    {

        // --- properties ---

        // --- constructors ---

        // --- methods ---
        public override void NewRuleParameters(Random random, SG_Shape ss) { }
        public override SG_Rule CopyRule(SG_Rule rule)
        {
            throw new NotImplementedException();
        }
        public override string RuleOperation(ref SG_Shape ss_ref, ref SG_Genotype gt)
        {
            throw new NotImplementedException();
        }
        public override State GetNextState()
        {
            throw new NotImplementedException();
        }
    }
}
