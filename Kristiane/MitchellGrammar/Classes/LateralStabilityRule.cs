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
    public class LateralStabilityRule : SH_Rule
    {
        // --- properties ---
        public int nrLateralStability;
        public int nrWall;

        // --- constructors ---
        public LateralStabilityRule()
        {
            Name = "LateralStabilityClass";
            RuleState = State.epsilon;
        }

        public LateralStabilityRule(int _nrLateralStability, int _nrWall)
        {
            nrLateralStability = _nrLateralStability;
            nrWall = _nrWall;
            Name = "LateralStabilityClass";
            RuleState = State.epsilon;
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
                return "The State is not compatible with LateralStabilityRule.";
            }


            // Same bracing for all substructures
            // ------------- nrLateralStability = 0 (Diagonal bracing) ---------- 
                if (nrLateralStability == 0)
                {
                    var columns = from col in _ss.Elements["Line"]
                                  where col.elementName.Contains("ColumnSub")
                                  select col;

                    //Point to compare with
                    Point3d pt = columns.ElementAt(0).Nodes[0].Position;
                    List<Line> linesToCompare = new List<Line>();
                    for (int c = 1; c < columns.Count(); c++)
                    {
                        if (pt.X == columns.ElementAt(c).Nodes[0].Position.X || pt.Y == columns.ElementAt(c).Nodes[0].Position.Y)
                        {
                            Line l = new Line(pt, columns.ElementAt(c).Nodes[1].Position);
                            linesToCompare.Add(l);
                        }
                        else
                        { 
                        SH_Element bracingElement = columns.ElementAt(c); //element diagonal from pt
                        }
                    }

                    

                    //find out which wall the bracing is located
                    if (linesToCompare[0].Length > linesToCompare[1].Length)
                    {
                        // ------------ (nrWall = 0) diagonal bracing on longitudinal wall ----------- 
                        if (nrWall == 0)
                        {
                        SH_Node[] bnodes = new SH_Node[2];
                        bnodes[0] = new SH_Node(linesToCompare[0].From, null);
                        bnodes[1] = new SH_Node(linesToCompare[0].To, null);

                        SH_Line sh_diagonalBrace = new SH_Line(bnodes, _ss.elementCount++, "DiagonalBraceWall0");
                        _ss.Elements["Line"].Add(sh_diagonalBrace);
                        }
                        // ------------ (nrWall = 1) diagonal bracing on transversal wall ----------- 
                        else if (nrWall == 1)
                        {
                            SH_Node[] bnodes = new SH_Node[2];
                            bnodes[0] = new SH_Node(linesToCompare[1].From, null);
                            bnodes[1] = new SH_Node(linesToCompare[1].To, null);

                            SH_Line sh_diagonalBrace = new SH_Line(bnodes, _ss.elementCount++, "DiagonalBraceWall1");
                            _ss.Elements["Line"].Add(sh_diagonalBrace);
                        }
                    }
                    else if (linesToCompare[0].Length < linesToCompare[1].Length)
                    {
                        // ------------ (nrWall = 0) diagonal bracing on longitudinal wall ----------- 
                        if (nrWall == 0)
                        {
                            SH_Node[] bnodes = new SH_Node[2];
                            bnodes[0] = new SH_Node(linesToCompare[1].From, null);
                            bnodes[1] = new SH_Node(linesToCompare[1].To, null);

                            SH_Line sh_diagonalBrace = new SH_Line(bnodes, _ss.elementCount++, "DiagonalBraceWall0");
                            _ss.Elements["Line"].Add(sh_diagonalBrace);
                        }
                        // ------------ (nrWall = 1) diagonal bracing on transversal wall ----------- 
                        else if (nrWall == 1)
                        {
                            SH_Node[] bnodes = new SH_Node[2];
                            bnodes[0] = new SH_Node(linesToCompare[0].From, null);
                            bnodes[1] = new SH_Node(linesToCompare[0].To, null);

                            SH_Line sh_diagonalBrace = new SH_Line(bnodes, _ss.elementCount++, "DiagonalBraceWall1");
                            _ss.Elements["Line"].Add(sh_diagonalBrace);
                        }
                    }
                }

            // change the state
            _ss.SimpleShapeState = State.zeta;
            return "LateralStability successfully applied.";

        }


        public override State GetNextState()
        {
            return State.zeta;
        }

    }
}
