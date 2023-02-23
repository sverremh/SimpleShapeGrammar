using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGrammar.Classes.Rules
{
    [Serializable]
    public class SG_AutoRule02 : SH_Rule 
    {
        // --- properties ---
        public List<string> ElemNames { get; set; } = new List<string>();
        // private readonly double[] bounds = { 0.2, 0.8 };

        // --- constructors ---
        public SG_AutoRule02()
        {
        }

        public SG_AutoRule02(List<string> _eNames)
        {
            RuleState = State.alpha;
            Name = "SH_AutoRule_02";
            ElemNames = _eNames;
        }

        // --- methods ---

        public override void NewRuleParameters(Random random, SG_Shape ss) { }
        public override SH_Rule CopyRule(SH_Rule rule)
        {
            throw new NotImplementedException();
        }
        public override string RuleOperation(ref SG_Shape ss_ref, ref SG_Genotype gt)
        {
            // find relevant range in genotype
            int sid = -999;
            int eid = -999;
            List<int> selectedIntGenes;
            List<double> selectedDGenes;

            gt.FindRange(ref sid, ref eid, Util.RULE01_MARKER);

            if (sid == -999 || eid == -999)
            {
                return "Autorule02 - wrong marker";
            }

            // extract relevant genes
            selectedIntGenes = gt.IntGenes.GetRange(sid, eid - sid);
            selectedDGenes = gt.DGenes.GetRange(sid, eid - sid);

            for (int i = 0; i < selectedIntGenes.Count; i++)
            {
                if (selectedIntGenes[i] == 0) continue;
                if (i >= ss_ref.Elems.Count) break;


            }

            return "";
        }
        public override State GetNextState()
        {
            throw new NotImplementedException();
        }
    }

}
