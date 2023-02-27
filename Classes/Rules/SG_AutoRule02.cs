﻿using System;
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
    public class SG_AutoRule02 : SG_Rule 
    {
        // --- properties ---
        public List<string> ElemNames { get; set; } = new List<string>();
        public double[] Domain { get; set; }

        // --- constructors ---
        public SG_AutoRule02()
        {
        }

        public SG_AutoRule02(List<string> _eNames, double[] _domain)
        {
            RuleState = State.alpha;
            Name = "SH_AutoRule_02";
            ElemNames = _eNames;
            Domain = _domain;

        }

        // --- methods ---

        public override void NewRuleParameters(Random random, SG_Shape ss) { }
        public override SG_Rule CopyRule(SG_Rule rule)
        {
            throw new NotImplementedException();
        }
        public override string RuleOperation(ref SG_Shape ss_ref, ref SG_Genotype gt)
        {
            string returnMessage = "";

            // find relevant range in genotype
            int sid = -999;
            int eid = -999;
            List<int> selectedIntGenes;
            List<double> selectedDGenes;

            gt.FindRange(ref sid, ref eid, Util.RULE02_MARKER);

            if (sid == -999 || eid == -999)
            {
                return "Autorule02 - wrong marker";
            }

            // extract relevant genes
            selectedIntGenes = gt.IntGenes.GetRange(sid, eid - sid);
            selectedDGenes = gt.DGenes.GetRange(sid, eid - sid);

            double range = Domain[1] - Domain[0];

            // remove unused nodes
            ss_ref.UnregisterElemsFromNodes();
            ss_ref.RegisterElemsToNodes();

            for (int i = 0; i < selectedIntGenes.Count; i++)
            {
                if (selectedIntGenes[i] == 0) continue;
                if (i >= ss_ref.Nodes.Count) break;

                double length = selectedDGenes[i] * range + Domain[0];

                if (Math.Abs(length) >= Util.MIN_SEG_LEN)
                {
                    Line ln = new Line(ss_ref.Nodes[i].Pt, Vector3d.ZAxis, length);
                    SG_Elem1D elem = new SG_Elem1D(ln, -999, "AR2", new SH_CrossSection_Beam()) { Autorule = 2 };
                    ss_ref.AddNewElement(elem);
                }
                else
                {
                    returnMessage += $"line too short: at node {ss_ref.Nodes[i].ID}\n";
                }

            }

            returnMessage += "Auto-rule 02 successfully applied.";


            return returnMessage;
        }
        public override State GetNextState()
        {
            throw new NotImplementedException();
        }
    }

}