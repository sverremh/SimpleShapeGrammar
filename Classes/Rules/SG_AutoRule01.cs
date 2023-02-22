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
    public class SG_AutoRule01 : SH_Rule
    {
        // --- properties ---

        // from parent class
        // public State RuleState;
        // public string Name;

        // from this class
        
        public int EID { get; set; }
        public double T { get; set; }
        public List<string> ElemNames { get; set; } = new List<string>();
        private double[] bounds = { 0.2, 0.8 };

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
        public override SH_Rule CopyRule(SH_Rule rule) 
        {
            throw new NotImplementedException();
        }
        public override string RuleOperation(ref SG_Shape ss_ref, ref SG_Genotype gt) 
        {
            // find relevant range in genotype
            int sid = -999;
            int eid = -999;

            for (int i = 0; i < gt.IntGenes.Count; i++)
            { 
                
            }


            // 


            SH_Elem1D elem = ss_ref.Elems.Where(e => e.ID == EID).First() as SH_Elem1D; 

            double seglen1 = elem.Ln.Length * T;
            double seglen2 = elem.Ln.Length * (1 - T);

            if (seglen1 < Util.MIN_SEG_LEN || seglen2 < Util.MIN_SEG_LEN)
            {
                return "Segments are too short for Autorule01.";
            }

            // add intermediate node
            SH_Node newNode = AddNode(elem, T, ss_ref.nodeCount);
            ss_ref.Nodes.Add(newNode);
            ss_ref.nodeCount++;

            // create 2x Element
            SH_Elem1D newLn0 = new SH_Elem1D(new SH_Node[] { elem.Nodes[0], newNode }, elem.ID, elem.Name);
            SH_Elem1D newLn1 = new SH_Elem1D(new SH_Node[] { newNode, elem.Nodes[1] }, ss_ref.elementCount, elem.Name);

            ss_ref.elementCount++;

            // remove Element just split
            int at = ss_ref.Elems.IndexOf(elem);
            ss_ref.Elems.RemoveAt(at);
            ss_ref.Elems.InsertRange(at, new List<SH_Element>() { newLn0, newLn1 } );

            return "auto rule 01 applied.";
        }
        public override State GetNextState() 
        {
            throw new NotImplementedException();
        }

        // methods of this class
        private SH_Node AddNode(SH_Element _e, double _t, int _id)
        {
            double mx = (1 - _t) * _e.Nodes[0].Pt.X + _t * _e.Nodes[1].Pt.X;
            double my = (1 - _t) * _e.Nodes[0].Pt.Y + _t * _e.Nodes[1].Pt.Y;
            double mz = (1 - _t) * _e.Nodes[0].Pt.Z + _t * _e.Nodes[1].Pt.Z;
            Point3d newPoint = new Point3d(mx, my, mz);

            return new SH_Node(newPoint, _id);
        }
    }
}
