using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleShapeGrammar.Classes;
using Rhino.Geometry;
using SimpleShapeGrammar.Classes.Elements;


namespace SimpleShapeGrammar.Kristiane.MitchellGrammar
{
    [Serializable]
    public  class BrepToSurfaceRule : SH_Rule
    {
        // --- properties ---
       
        // --- constructors ---
        public BrepToSurfaceRule()
        {
            Name = "BrepSurfaceClass";
            RuleState = State.alpha;
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
            var sh_solid = (SH_Solid)_ss.Elements["Solid"][0];

            // Transform a brep into surfaces
            var faceLst = sh_solid.Brep.Faces;
            _ss.Elements["Surface"] = new List<SH_Element>(); //empty list
            List<Surface> walls = new List<Surface>(); // empyt wall list
            //List<Surface> sortedWalls = new List<Surface>();
            //List<double> areas = new List<double>();
            foreach (BrepFace face in faceLst)
            {
                var srf = face.ToNurbsSurface();
                srf.SetDomain(0, new Interval(0, 1));
                srf.SetDomain(1, new Interval(0, 1));

                Vector3d v = srf.NormalAt(0.5, 0.5);
                // Check if normal vector points outwards from brep
                Point3d midpt = srf.PointAt(0.5, 0.5);
                Point3d checkpt = Point3d.Add(midpt, v);

                if (sh_solid.Brep.IsPointInside(checkpt, 0, true))
                {
                    v.Reverse(); // Flips the normal vector if the point is inside of the brep
                }       

                if (v.Z > 0)
                {
                    SH_Surface sh_srf = new SH_Surface(srf, "Top");
                    _ss.Elements["Surface"].Add(sh_srf);
                }
                else if (v.Z < 0)
                {
                    SH_Surface sh_srf = new SH_Surface(srf, "Bottom");
                    _ss.Elements["Surface"].Add(sh_srf);
                }
                else if (v.Z == 0)
                {
                    walls.Add(srf);
                    //double area = AreaMassProperties.Compute(srf).Area;
                    //areas.Add(area);   
                }
                
            }

            // Sort wall into transversal wall = smallest wall, and logintudinal wall = longest wall
            var sortedWalls = walls.OrderBy(w => AreaMassProperties.Compute(w).Area);
            for (int i = 0;  i < sortedWalls.Count() + 1; i++)
            {
                if (i == 0 || i == 1)
                {
                    Surface s = sortedWalls.ElementAt(i);
                    SH_Surface sh_srf = new SH_Surface(s, "Shortest Wall");
                    _ss.Elements["Surface"].Add(sh_srf);
                }
                else if (i == 2 || i == 3)
                {
                    Surface s = sortedWalls.ElementAt(i);
                    SH_Surface sh_srf = new SH_Surface(s, "Longest Wall");
                    _ss.Elements["Surface"].Add(sh_srf);
                }
            }

            /*
            foreach (Surface s in walls)
            {
                double max = areas.Max();

                if (AreaMassProperties.Compute(s).Area >= max)
                {
                    SH_Surface sh_srf = new SH_Surface(s, "Longest Wall");
                    _ss.Elements["Surface"].Add(sh_srf);
                }
                else if (AreaMassProperties.Compute(s).Area < max)
                {
                    SH_Surface sh_srf = new SH_Surface(s, "Shortest Wall");
                    _ss.Elements["Surface"].Add(sh_srf);
                }
            }
            */

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
