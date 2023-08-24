using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

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
            
            List<SG_Node> nds_side0 = selElems.Where(e => (e.Nodes[1].Pt.Z - e.Nodes[0].Pt.Z) > 0).Select(e => e.Nodes[1]).ToList();
            List<SG_Node> nds_side1 = selElems.Where(e => (e.Nodes[1].Pt.Z - e.Nodes[0].Pt.Z) < 0).Select(e => e.Nodes[1]).ToList();

            List<SG_Node> supNds = ss_ref.Nodes.Where(n => n.Support.SupportCondition > 0).ToList();

            foreach (var sn in supNds)
            {
                var r2Elem_from_supNds = sn.Elements.Where(e => e.Autorule == 2);

                if (r2Elem_from_supNds.Count() == 0)
                {
                    nds_side0.Add(sn);
                    nds_side1.Add(sn);
                }

                else
                {
                    var r2_elem = r2Elem_from_supNds.First();

                    if (r2_elem.Nodes[1].Pt.Z - r2_elem.Nodes[0].Pt.Z > 0) 
                    {
                        nds_side1.Add(sn);
                    }
                    else
                    {
                        nds_side0.Add(sn);
                    }
                }
            }

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

                if (Math.Abs(nd0.Pt.X - nd1.Pt.X) < UT.PRES)
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

                if (Math.Abs(nd0.Pt.X - nd1.Pt.X) < UT.PRES)
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

            // add R2 elems
            // 2023.08.24

            List<int> removeIds = new List<int>();
            var initialR3s = ss_ref.Elems.Where(e => e.Autorule == 3).ToList();
            foreach (SG_Elem1D r3e in initialR3s)
            {

                var selNds = ss_ref.Nodes.Where(n => n.Pt.X > r3e.Nodes[0].Pt.X && n.Pt.X < r3e.Nodes[1].Pt.X).ToList();
                foreach (var nd in selNds)
                {
                    // unselect irrelevant nodes
                    if (nd.Elements.Where(e => e.Autorule == 1).Count() == 0) continue;

                    // find intersection
                    var vL = new Line(nd.Pt, Vector3d.ZAxis);
                    Intersection.LineLine(r3e.Ln, vL, out double param, out _);

                    // add intermediate node
                    SG_Node midNode = SG_Node.CreateNode(r3e, param, ss_ref.nodeCount);
                    ss_ref.Nodes.Add(midNode);
                    ss_ref.nodeCount++;

                    // create one vertical elem and two R3 elem by splitting the initial R3 elem
                    SG_Elem1D newVL = new SG_Elem1D(new SG_Node[2] { nd, midNode }, ss_ref.elementCount, "AR2") { Autorule = 3 };
                    SG_Elem1D newR3_0 = new SG_Elem1D(new SG_Node[2] { r3e.Nodes[0], midNode }, ss_ref.elementCount + 1, r3e.Name) { Autorule = 3 };
                    SG_Elem1D newR3_1 = new SG_Elem1D(new SG_Node[2] { midNode, r3e.Nodes[1] }, ss_ref.elementCount + 2, r3e.Name) { Autorule = 3 };

                    ss_ref.elementCount += 3;

                    // remove element just split
                    removeIds.Add(r3e.ID);

                    // add three elements to shape
                    ss_ref.Elems.AddRange(new List<SG_Element>(3) { newVL, newR3_0, newR3_1 });

                }

            }

            ss_ref.Elems = ss_ref.Elems.Where(e => removeIds.Contains(e.ID) == false).ToList();

            return "Auto-rule 03 successfully applied.";
        }
        public override State GetNextState()
        {
            throw new NotImplementedException();
        }

        // private void 
    }
}
