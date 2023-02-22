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
    public class SH_AutoRule01 : SH_Rule
    {
        // --- properties ---

        // from parent class
        // public State RuleState;
        // public string Name;

        public int EID { get; set; }
        public double T { get; set; }
        private double[] bounds = { 0.2, 0.8 };

        // --- constructors --- 

        public SH_AutoRule01()
        {
            RuleState = State.alpha;
            Name = "SH_AutoRule_01";
        }

        public SH_AutoRule01(int _eid, double _t)
        {
            RuleState = State.alpha;
            Name = "SH_AutoRule_01";

            EID = _eid;
            T = _t;
            
        }

        // --- methods ---
        // methods of parent class
        public override void NewRuleParameters(Random random, SH_SimpleShape ss) { }
        public override SH_Rule CopyRule(SH_Rule rule) 
        {
            throw new NotImplementedException();
        }
        public override string RuleOperation(ref SH_SimpleShape _ss) 
        {
            SH_Elem1D elem = _ss.Elems.Where(e => e.ID == EID).First() as SH_Elem1D; 

            double seglen1 = elem.Ln.Length * T;
            double seglen2 = elem.Ln.Length * (1 - T);

            if (seglen1 < Util.MIN_SEG_LEN || seglen2 < 1.0)
            {
                return "Segments are too short for Autorule01.";
            }

            // add intermediate node
            SH_Node newNode = AddNode(elem, T, _ss.nodeCount);
            _ss.Nodes.Add(newNode);
            _ss.nodeCount++;

            // create 2x Element
            SH_Elem1D newLn0 = new SH_Elem1D(new SH_Node[] { elem.Nodes[0], newNode }, elem.ID, elem.elementName);
            SH_Elem1D newLn1 = new SH_Elem1D(new SH_Node[] { newNode, elem.Nodes[1] }, _ss.elementCount, elem.elementName);

            _ss.elementCount++;

            // remove Element just split
            int at = _ss.Elems.IndexOf(elem);
            _ss.Elems.RemoveAt(at);
            _ss.Elems.InsertRange(at, new List<SH_Element>() { newLn0, newLn1 } );

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
