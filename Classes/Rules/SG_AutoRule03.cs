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
    public class SG_AutoRule03 : SG_Rule
    {

        // --- properties ---
        public string ElemName { get; set; }

        // --- constructors ---
        public SG_AutoRule03()
        {
        }

        public SG_AutoRule03(string _eName)
        {
            RuleState = State.alpha;
            Name = "SH_AutoRule_03";
            ElemName = _eName;

        }

        // --- methods ---
        public override void NewRuleParameters(Random random, SG_Shape ss) { }
        public override SG_Rule CopyRule(SG_Rule rule)
        {
            throw new NotImplementedException();
        }
        public override string RuleOperation(ref SG_Shape ss_ref, ref SG_Genotype gt)
        {

            var selElems = ss_ref.Elems.Where(e => e.Autorule == 2);
            List<SG_Node> selNodes = ss_ref.Nodes.Where(n => n.Support.SupportCondition > 0).ToList();
            
            //foreach (SG_Node n in ss_ref.Nodes)
            //{
            //    var Autorules = n.Elements.Select(e => e.Autorule);
            //    if (Autorules.Contains(2)) continue;
            //    if (n.Support.SupportCondition > 0)
            //    {
            //        selNodes.Add(n);
            //    }
            //}

            List<SG_Node> nds_side0 = selElems.Where(e => (e.Nodes[1].Pt.Z - e.Nodes[0].Pt.Z) > 0).Select(e => e.Nodes[1]).ToList();
            List<SG_Node> nds_side1 = selElems.Where(e => (e.Nodes[1].Pt.Z - e.Nodes[0].Pt.Z) < 0).Select(e => e.Nodes[1]).ToList();

            nds_side0.AddRange(selNodes);
            nds_side1.AddRange(selNodes);

            List<SG_Node> nds_side0_sorted = nds_side0.OrderBy(n => n.Pt.X).ToList();
            List<SG_Node> nds_side1_sorted = nds_side1.OrderBy(n => n.Pt.X).ToList();

            for (int i = 0; i < nds_side0_sorted.Count-1; i++)
            {
                SG_Node nd0 = nds_side0_sorted[i];
                SG_Node nd1 = nds_side0_sorted[i + 1];

                if (nd0.Support.SupportCondition > 0 && nd1.Support.SupportCondition > 0)
                {
                    continue;
                }

                if (Math.Abs(nd0.Pt.X - nd1.Pt.X) < Util.PRES)
                {
                    if (i == nds_side0_sorted.Count - 2) continue;
                    else
                    {
                        nd1 = nds_side0_sorted[i + 2];
                        i++;
                    }
                }
                
                SG_Node[] nds = new SG_Node[2] { nd0, nd1 };
                SG_Elem1D newElem = new SG_Elem1D(nds, -999, ElemName) { Autorule = 3};

                ss_ref.AddNewElement(newElem);
            }

            for (int i = 0; i < nds_side1_sorted.Count - 1; i++)
            {
                SG_Node nd0 = nds_side1_sorted[i];
                SG_Node nd1 = nds_side1_sorted[i + 1];

                if (nd0.Support.SupportCondition > 0 && nd1.Support.SupportCondition > 0)
                {
                    continue;
                }

                if (Math.Abs(nd0.Pt.X - nd1.Pt.X) < Util.PRES)
                {
                    if (i == nds_side1_sorted.Count - 2) continue;
                    else
                    {
                        nd1 = nds_side1_sorted[i + 2];
                        i++;
                    }
                }

                SG_Node[] nds = new SG_Node[2] { nd0, nd1 };
                SG_Elem1D newElem = new SG_Elem1D(nds, -999, ElemName) { Autorule = 3};

                ss_ref.AddNewElement(newElem);
            }


            return "";
        }
        public override State GetNextState()
        {
            throw new NotImplementedException();
        }

        // private void 
    }
}
