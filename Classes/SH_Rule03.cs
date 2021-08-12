using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace SimpleShapeGrammar.Classes
{
    [Serializable]
    public class SH_Rule03 : SH_Rule
    {
        // --- properties ---
        public double HorizontalThrust { get; set; }
        public bool Compression { get; set; }

        public State RuleState = State.gamma;

        // --- constructors ---
        public SH_Rule03()
        {
            // empty
        }
        public SH_Rule03(double _thrust, bool _compression)
        {
            HorizontalThrust = _thrust;
            Compression = _compression;
        }

        // --- methods ---
        public override void RuleOperation(SH_SimpleShape _ss)
        {
            // --- solve ---
            SH_Evaluation.ConstructMatrices(_ss, out double[,] a, out double[] b);

            // calculate moments over the supports
            double[] moments = SH_Evaluation.CalculateMoments(a, b);
            double[] forces = SH_Evaluation.CalculateForces(_ss, moments);
            double thrust = 50; // make this a user specified input later.
            double[] reactions = SH_Evaluation.CalculateReactions(_ss, forces, thrust);

            // draw reciprocal diagram
            Dictionary<string, List<Line>> reciprocal_diagram = new Dictionary<string, List<Line>>();
            try
            {
                reciprocal_diagram = SH_Evaluation.DrawReciprocal(_ss, reactions, forces, thrust);
                
            }
            catch (Exception ex)
            {
                
                //AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not enough elements to draw reciprocal. Minimum number is 2.");
            }


            // change state til "end" 
            _ss.SimpleShapeState = State.end;
        }
    }
}
