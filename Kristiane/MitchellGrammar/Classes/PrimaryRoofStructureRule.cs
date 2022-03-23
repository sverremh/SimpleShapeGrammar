using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleShapeGrammar.Classes;
using SimpleShapeGrammar.Classes.Elements;
using Rhino.Geometry;


namespace SimpleShapeGrammar.Kristiane.MitchellGrammar
{
    [Serializable]
    public class PrimaryRoofStructureRule : SH_Rule
    {
        // --- properties ---


        // --- constructors ---
        public PrimaryRoofStructureRule()
        {
            Name = "PrimaryRoofStructureClass";
            RuleState = State.gamma;
        }

        // --- methods ---

        public override SH_Rule CopyRule(SH_Rule rule)
        {
            throw new NotImplementedException();
        }

        public override void NewRuleParameters(Random random, SH_SimpleShape ss)
        {
            throw new NotImplementedException();
        }

        public override string RuleOperation(SH_SimpleShape _ss)
        {
            // Get the substructure number from MRule2
            var nrSub2 = new SubStructureRule();
            double nrSub = nrSub2.nrSub;

            // ------------- Primary roof structure for substructure 0 ------------- 
            if (nrSub == 0)
            {
                List<SH_Element> lineLst = _ss.Elements["Line"];

                foreach (SH_Line line in lineLst)
                {
                    SH_Line sh_line = (SH_Line)line;
                    string n = sh_line.elementName;
                }
            }
            // ------------- Primary roof structure for substructure 1 ------------- 
            // ------------- Primary roof structure for substructure 2 ------------- (pitched roof)
            // ------------- Primary roof structure for substructure 3 ------------- (bowed roof)
            if (nrSub == 3)
            {
                List<SH_Element> lineLst = _ss.Elements["Line"];

                foreach (SH_Line line in lineLst)
                {
                    SH_Line sh_line = (SH_Line)line;
                    string n = sh_line.elementName;
                }
            }
            


            // change the state
            _ss.SimpleShapeState = State.delta;
            return "BrepToSurface successfully applied.";
        }

        public override State GetNextState()
        {
            return State.delta;
        }

    }
}
