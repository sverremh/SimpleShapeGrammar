using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

using ShapeGrammar.Classes;
using ShapeGrammar.Classes.Elements;

namespace ShapeGrammar.Classes.Rules
{
    [Serializable]
    public class SG_AutoRule01 : SG_Rule
    {
        // --- properties ---

        // from parent class
        // public State RuleState;
        // public string Name;

        // from this class
        
        // public int EID { get; set; }
        // public double T { get; set; }
        public List<string> ElemNames { get; set; } = new List<string>();
        private readonly double[] bounds = { 0.2, 0.8 };

        // --- constructors --- 

        public SG_AutoRule01()
        {
            RuleState = State.alpha;
            Name = "SH_AutoRule_01";
        }

        public SG_AutoRule01(List<string> _eNames)
        {
            RuleState = State.alpha;
            Name = "SH_AutoRule_01";
            ElemNames = _eNames;

            // T = _t;
            
        }

        // --- methods ---
        // methods of parent class
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

            gt.FindRange(ref sid, ref eid, Util.RULE01_MARKER);

            if (sid == -999 || eid == -999)
            {
                return "Autorule01 - wrong marker";
            }

            // extract relevant genes
            selectedIntGenes = gt.IntGenes.GetRange(sid, eid - sid);
            selectedDGenes = gt.DGenes.GetRange(sid, eid - sid);

            List<int> removeIds = new List<int>();
            for (int i = 0; i < selectedIntGenes.Count; i++)
            {
                if (selectedIntGenes[i] == 0) continue;
                if (i >= ss_ref.Elems.Count) break;

                SG_Elem1D elem = ss_ref.Elems[i] as SG_Elem1D;
                double param = selectedDGenes[i];
                if (param < bounds[0])
                {
                    param = bounds[0];
                }
                else if (param > bounds[1])
                {
                    param = bounds[1];
                }

                double seglen1 = elem.Ln.Length * param;
                double seglen2 = elem.Ln.Length * (1 - param);

                if (seglen1 < Util.MIN_SEG_LEN || seglen2 < Util.MIN_SEG_LEN)
                {
                    return "Segments are too short for Autorule01.";
                }

                // add intermediate node
                SG_Node newNode = AddNode(elem, param, ss_ref.nodeCount);
                ss_ref.Nodes.Add(newNode);
                ss_ref.nodeCount++;

                // create 2x Element
                SG_Elem1D newLn0 = new SG_Elem1D(new SG_Node[] { elem.Nodes[0], newNode }, ss_ref.elementCount, elem.Name) { Autorule = 1};
                SG_Elem1D newLn1 = new SG_Elem1D(new SG_Node[] { newNode, elem.Nodes[1] }, ss_ref.elementCount+1, elem.Name) { Autorule = 1 };

                ss_ref.elementCount += 2;

                // remove Element just split
                // int at = ss_ref.Elems.IndexOf(elem);
                // ss_ref.Elems.RemoveAt(at);
                removeIds.Add(elem.ID);
                ss_ref.Elems.AddRange(new List<SG_Element>() { newLn0, newLn1 });
                // ss_ref.Elems.InsertRange(at, new List<SG_Element>() { newLn0, newLn1 });
            }

            ss_ref.Elems = ss_ref.Elems.Where(e => removeIds.Contains(e.ID) == false).ToList();

            return "Auto-rule 01 successfully applied.";

        }
        public override State GetNextState() 
        {
            throw new NotImplementedException();
        }

        // methods of this class
        private SG_Node AddNode(SG_Element _e, double _t, int _id)
        {
            double mx = (1 - _t) * _e.Nodes[0].Pt.X + _t * _e.Nodes[1].Pt.X;
            double my = (1 - _t) * _e.Nodes[0].Pt.Y + _t * _e.Nodes[1].Pt.Y;
            double mz = (1 - _t) * _e.Nodes[0].Pt.Z + _t * _e.Nodes[1].Pt.Z;
            Point3d newPoint = new Point3d(mx, my, mz);

            SG_Node nd = new SG_Node(newPoint, _id);

            return nd;
        }
    }
}
