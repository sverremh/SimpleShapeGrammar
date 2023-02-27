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
    public class SG_AutoRule04 : SG_Rule
    {

        // --- properties ---
        public string ElemName { get; set; }

        // --- constructors ---
        public SG_AutoRule04()
        {
        }

        public SG_AutoRule04(string _eName)
        {
            RuleState = State.alpha;
            Name = "SH_AutoRule_04";
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
            // find relevant range in genotype
            int sid = -999;
            int eid = -999;
            List<int> selectedIntGenes;
            List<double> selectedDGenes;

            gt.FindRange(ref sid, ref eid, Util.RULE04_MARKER);

            if (sid == -999 || eid == -999)
            {
                return "Autorule04 - wrong marker";
            }

            // extract relevant genes
            selectedIntGenes = gt.IntGenes.GetRange(sid, eid - sid);
            selectedDGenes = gt.DGenes.GetRange(sid, eid - sid);

            if (selectedIntGenes[0] == 0) return "Auto-rule 04 not applied.";

            // rule 4 content

            var r1_elems = ss_ref.Elems.Where(e => e.Autorule == 1).ToList();
            var r2_elems = ss_ref.Elems.Where(e => e.Autorule == 2).ToList();
            var r3_elems = ss_ref.Elems.Where(e => e.Autorule == 3).ToList();

            List<int> removeIds = new List<int>();
            foreach (var r2 in r2_elems)
            {
                foreach (var r3 in r3_elems)
                {
                    var intersect = Intersection.CurveLine(
                        (r3 as SG_Elem1D).Ln.ToNurbsCurve(), (r2 as SG_Elem1D).Ln, Util.PRES, Util.PRES);

                    if (intersect.Count == 0) continue;

                    if (intersect[0].PointA.DistanceToSquared(r2.Nodes[0].Pt) < Util.PRES) continue;
                    else if (intersect[0].PointA.DistanceToSquared(r2.Nodes[1].Pt) < Util.PRES) continue;

                    else
                    {
                        Intersection.LineLine((r3 as SG_Elem1D).Ln, (r2 as SG_Elem1D).Ln, out double param, out _);

                        // add intermediate node
                        SG_Node midNode = SG_Node.CreateNode(r3, param, ss_ref.nodeCount);
                        ss_ref.Nodes.Add(midNode);
                        ss_ref.nodeCount++;

                        // create 3x Element
                        SG_Elem1D newLn0 = new SG_Elem1D(new SG_Node[] { r3.Nodes[0], midNode }, ss_ref.elementCount, r3.Name) { Autorule = 3 };
                        SG_Elem1D newLn1 = new SG_Elem1D(new SG_Node[] { midNode, r3.Nodes[1] }, ss_ref.elementCount + 1, r3.Name) { Autorule = 3 };

                        SG_Elem1D newLn2 = new SG_Elem1D(new SG_Node[] { r2.Nodes[1], midNode }, ss_ref.elementCount + 2, r2.Name) { Autorule = 2 };

                        ss_ref.elementCount += 3;

                        // remove Element just split
                        removeIds.Add(r3.ID);
                        ss_ref.Elems.AddRange(new List<SG_Element>(3) { newLn0, newLn1, newLn2 });

                        // remove B1 
                        // functioning code below but need to be cleaned up later: 230226
                        var selr1elems0 = r1_elems.Where(r1 =>
                        0.5 * ((r1 as SG_Elem1D).Ln.FromX + (r1 as SG_Elem1D).Ln.ToX) > Math.Min(newLn0.Ln.FromX, newLn0.Ln.ToX) &&
                        0.5 * ((r1 as SG_Elem1D).Ln.FromX + (r1 as SG_Elem1D).Ln.ToX) < Math.Max(newLn0.Ln.FromX, newLn0.Ln.ToX));
                        foreach (SG_Element e in selr1elems0)
                        {
                            ss_ref.Elems.Remove(e);
                        }

                        // remove AR2
                        ss_ref.Elems.Remove(r2);

                        var selr1elems1 = r1_elems.Where(r1 =>
                        0.5 * ((r1 as SG_Elem1D).Ln.FromX + (r1 as SG_Elem1D).Ln.ToX) > Math.Min(newLn1.Ln.FromX, newLn1.Ln.ToX) &&
                        0.5 * ((r1 as SG_Elem1D).Ln.FromX + (r1 as SG_Elem1D).Ln.ToX) < Math.Max(newLn1.Ln.FromX, newLn1.Ln.ToX));
                        foreach (SG_Element e in selr1elems1)
                        {
                            ss_ref.Elems.Remove(e);
                        }

                    }
                }
            }

            ss_ref.Elems = ss_ref.Elems.Where(e => removeIds.Contains(e.ID) == false).ToList();

            // add diagonal

            r2_elems = ss_ref.Elems.Where(e => e.Autorule == 2).OrderBy(e => e.Nodes[0].Pt.X).ToList();

            for (int i = 0; i < r2_elems.Count-1; i++)
            {
                var e0n0 = r2_elems[i].Nodes[0];
                var e0n1 = r2_elems[i].Nodes[1];
                var e1n0 = r2_elems[i+1].Nodes[0];
                var e1n1 = r2_elems[i+1].Nodes[1];

                if (e0n0.Pt.Z < e0n1.Pt.Z)
                {
                    e0n0 = r2_elems[i].Nodes[1];
                    e0n1 = r2_elems[i].Nodes[0];
                }

                if (e1n0.Pt.Z < e1n1.Pt.Z)
                {
                    e1n0 = r2_elems[i + 1].Nodes[1];
                    e1n1 = r2_elems[i + 1].Nodes[0];
                }

                // create 2x Element
                SG_Elem1D newLn0 = new SG_Elem1D(new SG_Node[] { e0n0, e1n1 }, ss_ref.elementCount, "dg") { Autorule = 4 };
                SG_Elem1D newLn1 = new SG_Elem1D(new SG_Node[] { e0n1, e1n0 }, ss_ref.elementCount + 1, "dg") { Autorule = 4 };
                ss_ref.elementCount += 2;

                ss_ref.Elems.AddRange(new List<SG_Element>(2) { newLn0, newLn1});
            }

            // remove unused nodes
            ss_ref.UnregisterElemsFromNodes();
            ss_ref.RegisterElemsToNodes();
            ss_ref.RemoveUnusedNodes();

            return "Auto-rule 04 successfully applied.";
        }
        public override State GetNextState()
        {
            throw new NotImplementedException();
        }
    }
}
