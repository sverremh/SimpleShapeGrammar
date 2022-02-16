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
    public  class BrepToSurfaceRule : SH_Rule
    {
        // --- properties ---
        public List<Surface> srfLst = new List<Surface>();
        public List<string> nameLst = new List<string>();
        public Brep geo;
        public List<Vector3d> vecs = new List<Vector3d>();

        // --- constructors ---
        public BrepToSurfaceRule()
        {
            Name = "BrepSurfaceClass";
            RuleState = State.alpha;
        }

        public BrepToSurfaceRule(Brep _Geo)
        {
            Name = "BrepSurfaceClass";
            RuleState = State.alpha;
            //geo = _Geo;
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

            // Brep
            Brep sh_brep = _ss.Breps[0];

            // transform a brep into six surfaces
            //BrepToSurfaceRule geo = new BrepToSurfaceRule();
            var faceLst = sh_brep.Faces;
            //List<Surface> srfLst = new List<Surface>();
            foreach (BrepFace face in faceLst)
            {
                srfLst.Add(face.ToNurbsSurface());
            }

            //vector of each surface
            //List<Vector3d> vecs = new List<Vector3d>();
            foreach (Surface s in srfLst)
            {
                Vector3d vec = s.NormalAt(0.5, 0.5);
                vecs.Add(vec);
            }

            //name each surface
            //List<string> srfName = new List<string>();
            foreach (Vector3d v in vecs)
            {
                if (v.Z > 0)
                {
                    nameLst.Add("Top");
                }
                else if (v.Z < 0)
                {
                    nameLst.Add("Bottom");
                }
                else if (v.Z == 0)
                {
                    nameLst.Add("Wall");
                }

            }
            _ss.Surfaces = srfLst;


            // change the state
            _ss.SimpleShapeState = State.beta;
            return "BrepToSurface successfully applied.";
        }

        public override State GetNextState()
        {
            return State.beta;
        }

    }
}
