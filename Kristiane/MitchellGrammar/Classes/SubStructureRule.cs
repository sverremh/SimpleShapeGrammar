using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleShapeGrammar.Classes;
using Rhino.Geometry;

namespace SimpleShapeGrammar.Kristiane.MitchellGrammar
{
    [Serializable]
    public class SubStructureRule : SH_Rule
    {
        // --- properties ---
        public List<string> nameSubLSt = new List<string>();
        public double nrSub; //number of substructure
        public double t; // thickness


        // --- constructor --- 
        public SubStructureRule()
        {
            Name = "SubStructureClass";
            RuleState = State.beta;
        }

        public SubStructureRule(double _nrSub, double _t)
        {
            nrSub = _nrSub;
            t = _t;
            Name = "SubStructureClass";
            RuleState = State.beta;
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
            // check if the state maches the simple shape state
            if (_ss.SimpleShapeState != RuleState)
            {
                return "The State is not compatible with BrepToSurface.";
            }


            /*// SubStructure 0
            foreach ( Surface s in _ss.Surfaces)
            {
                foreach (Vector3d v in BrepToSurfaceRule.vecs)
                {
                    if (v.Z > 0 || v.Z == 0)
                    {
                        Surface s2 = s.Offset(-t / 2, 0.0001);
                        //Surface srf2 = s.Offset(t / 2, 0.0001);

                        Brep lSrf =
                    }
                    elseif
                }
            }*/


            // change the state
            _ss.SimpleShapeState = State.gamma;
            return "BrepToSurface successfully applied.";
        }

        public override State GetNextState()
        {
            return State.gamma;
        }

    }
}
