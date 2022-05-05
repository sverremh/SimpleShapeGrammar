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
        public List<int> nrLateralStability;
        public List<int> nrWall;
        public double distBrace;

        // --- constructors ---
        public LateralStabilityRule()
        {
            Name = "LateralStabilityClass";
            RuleState = State.epsilon;
        }

        public LateralStabilityRule(List<int> _nrLateralStability, List<int> _nrWall, double _distBrace)
        {
            nrLateralStability = _nrLateralStability;
            nrWall = _nrWall;
            distBrace = _distBrace;
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
            var columns = from col in _ss.Elements["Line"]
                          where col.elementName.Contains("Column")
                          select col;

            SH_Line sh_el = (SH_Line)columns.ElementAt(0);
            string matName = sh_el.CrossSection.Material.Name;
            SH_Material beamMat = new SH_Material(matName);
            string cSec = sh_el.CrossSection.Name;

            SH_Element col1 = columns.ElementAt(0); //one column
            SH_Element col2 = new SH_Line(); //diagonal column of col1
            List<SH_Element> cols = new List<SH_Element>(); //list with the two last columns, that are diagonal from each other
            for (int c = 1; c < columns.Count(); c++)
            {
                if (col1.Nodes[0].Position.X == columns.ElementAt(c).Nodes[0].Position.X || col1.Nodes[0].Position.Y == columns.ElementAt(c).Nodes[0].Position.Y)
                {
                    cols.Add(columns.ElementAt(c));
                }
                else
                {
                    col2 = columns.ElementAt(c);
                }
            }

            List<SH_Node> nodeLst = new List<SH_Node>();  // Store nodes
            List<int?> IDLst = new List<int?>(); // Empty list to store ID that will remove corresponding element removed later
            List<string> nameLst = new List<string>(); //Empty list to store element names to remove corresponding element later

            for (int numberLat = 0; numberLat < nrWall.Count(); numberLat++)
            {
                // ------------- nrLateralStability = 0 (DIAGONAL BRACING) ----------
                if (nrLateralStability.ElementAt(numberLat) == 0)
                {
                    if (nrWall.ElementAt(numberLat) == 0)
                    {
                        Line brace = new Line(col1.Nodes[1].Position, cols.ElementAt(0).Nodes[0].Position);
                        SH_Node[] bnodes = new SH_Node[2];
                        bnodes[0] = new SH_Node(brace.From, null);
                        bnodes[1] = new SH_Node(brace.To, null);
                        SH_Line sh_diagonalBrace = new SH_Line(bnodes, _ss.elementCount++, "DiagonalBraceWall0");
                        sh_diagonalBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_diagonalBrace);
                    }
                    else if (nrWall.ElementAt(numberLat) == 1)
                    {
                        Line brace = new Line(col2.Nodes[0].Position, cols.ElementAt(1).Nodes[1].Position);
                        SH_Node[] bnodes = new SH_Node[2];
                        bnodes[0] = new SH_Node(brace.From, null);
                        bnodes[1] = new SH_Node(brace.To, null);
                        SH_Line sh_diagonalBrace = new SH_Line(bnodes, _ss.elementCount++, "DiagonalBraceWall3");
                        sh_diagonalBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_diagonalBrace);
                    }
                    else if (nrWall.ElementAt(numberLat) == 2)
                    {
                        Line brace = new Line(col1.Nodes[0].Position, cols.ElementAt(1).Nodes[1].Position);
                        SH_Node[] bnodes = new SH_Node[2];
                        bnodes[0] = new SH_Node(brace.To, null);
                        bnodes[1] = new SH_Node(brace.From, null);
                        SH_Line sh_diagonalBrace = new SH_Line(bnodes, _ss.elementCount++, "DiagonalBraceWall1");
                        sh_diagonalBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_diagonalBrace);
                    }
                    else if (nrWall.ElementAt(numberLat) == 3)
                    {
                        Line brace = new Line(col2.Nodes[1].Position, cols.ElementAt(0).Nodes[0].Position);
                        SH_Node[] bnodes = new SH_Node[2];
                        bnodes[0] = new SH_Node(brace.To, null);
                        bnodes[1] = new SH_Node(brace.From, null);
                        SH_Line sh_diagonalBrace = new SH_Line(bnodes, _ss.elementCount++, "DiagonalBraceWall2");
                        sh_diagonalBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_diagonalBrace);
                    }

                }

                // ------------- nrLateralStability = 1 (CROSS BRACING) ----------
                if (nrLateralStability.ElementAt(numberLat) == 1)
                {
                    if (nrWall.ElementAt(numberLat) == 0)
                    {
                        Line cbrace1 = new Line(col1.Nodes[0].Position, cols.ElementAt(0).Nodes[1].Position);
                        SH_Node[] bnodes1 = new SH_Node[2];
                        bnodes1[0] = new SH_Node(cbrace1.From, null);
                        bnodes1[1] = new SH_Node(cbrace1.To, null);
                        SH_Line sh_crossBrace1 = new SH_Line(bnodes1, _ss.elementCount++, "CrossBraceWall0");
                        sh_crossBrace1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_crossBrace1);

                        Line cbrace2 = new Line(col1.Nodes[1].Position, cols.ElementAt(0).Nodes[0].Position);
                        SH_Node[] bnodes2 = new SH_Node[2];
                        bnodes2[0] = new SH_Node(cbrace2.From, null);
                        bnodes2[1] = new SH_Node(cbrace2.To, null);
                        SH_Line sh_crossBrace2 = new SH_Line(bnodes2, _ss.elementCount++, "CrossBraceWall0");
                        sh_crossBrace2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_crossBrace2);
                    }
                    else if (nrWall.ElementAt(numberLat) == 1)
                    {
                        Line cbrace1 = new Line(col1.Nodes[0].Position, cols.ElementAt(1).Nodes[1].Position);
                        SH_Node[] bnodes1 = new SH_Node[2];
                        bnodes1[0] = new SH_Node(cbrace1.From, null);
                        bnodes1[1] = new SH_Node(cbrace1.To, null);
                        SH_Line sh_crossBrace1 = new SH_Line(bnodes1, _ss.elementCount++, "CrossBraceWall1");
                        sh_crossBrace1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_crossBrace1);

                        Line cbrace2 = new Line(col1.Nodes[1].Position, cols.ElementAt(1).Nodes[0].Position);
                        SH_Node[] bnodes2 = new SH_Node[2];
                        bnodes2[0] = new SH_Node(cbrace2.From, null);
                        bnodes2[1] = new SH_Node(cbrace2.To, null);
                        SH_Line sh_crossBrace2 = new SH_Line(bnodes2, _ss.elementCount++, "CrossBraceWall1");
                        sh_crossBrace2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_crossBrace2);
                    }
                    else if (nrWall.ElementAt(numberLat) == 2)
                    {
                        Line cbrace1 = new Line(col2.Nodes[0].Position, cols.ElementAt(0).Nodes[1].Position);
                        SH_Node[] bnodes1 = new SH_Node[2];
                        bnodes1[0] = new SH_Node(cbrace1.From, null);
                        bnodes1[1] = new SH_Node(cbrace1.To, null);
                        SH_Line sh_crossBrace1 = new SH_Line(bnodes1, _ss.elementCount++, "CrossBraceWall2");
                        sh_crossBrace1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_crossBrace1);

                        Line cbrace2 = new Line(col2.Nodes[1].Position, cols.ElementAt(0).Nodes[0].Position);
                        SH_Node[] bnodes2 = new SH_Node[2];
                        bnodes2[0] = new SH_Node(cbrace2.From, null);
                        bnodes2[1] = new SH_Node(cbrace2.To, null);
                        SH_Line sh_crossBrace2 = new SH_Line(bnodes2, _ss.elementCount++, "CrossBraceWall2");
                        sh_crossBrace2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_crossBrace2);
                    }
                    else if (nrWall.ElementAt(numberLat) == 3)
                    {
                        Line cbrace1 = new Line(col2.Nodes[0].Position, cols.ElementAt(1).Nodes[1].Position);
                        SH_Node[] bnodes1 = new SH_Node[2];
                        bnodes1[0] = new SH_Node(cbrace1.From, null);
                        bnodes1[1] = new SH_Node(cbrace1.To, null);
                        SH_Line sh_crossBrace1 = new SH_Line(bnodes1, _ss.elementCount++, "CrossBraceWall3");
                        sh_crossBrace1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_crossBrace1);

                        Line cbrace2 = new Line(col2.Nodes[1].Position, cols.ElementAt(1).Nodes[0].Position);
                        SH_Node[] bnodes2 = new SH_Node[2];
                        bnodes2[0] = new SH_Node(cbrace2.From, null);
                        bnodes2[1] = new SH_Node(cbrace2.To, null);
                        SH_Line sh_crossBrace2 = new SH_Line(bnodes2, _ss.elementCount++, "CrossBraceWall3");
                        sh_crossBrace2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_crossBrace2);
                    }
                }

                // ------------- nrLateralStability = 2 (KNEE BRACES) ----------
                if (nrLateralStability.ElementAt(numberLat) == 2)
                {
                    var transBeam = from tBeam in _ss.Elements["Line"]
                                    where tBeam.elementName.Contains("Trans")
                                    select tBeam;

                    var longBeam = from lBeam in _ss.Elements["Line"]
                                   where lBeam.elementName.Contains("Longitudinal")
                                   select lBeam;
                    // Wall 0
                    if (nrWall.ElementAt(numberLat) == 0)
                    {
                        //First knee brace
                        Point3d pt1 = col1.Nodes[1].Position;
                        Point3d pt11 = new Point3d(pt1.X + distBrace, pt1.Y, pt1.Z);
                        Point3d pt12 = new Point3d(pt1.X, pt1.Y, pt1.Z - distBrace);

                        Line kbrace1 = new Line(pt11, pt12);
                        SH_Node[] knodes1 = new SH_Node[2];
                        knodes1[0] = new SH_Node(kbrace1.From, null);
                        knodes1[1] = new SH_Node(kbrace1.To, null);
                        SH_Line sh_kneeBrace1 = new SH_Line(knodes1, _ss.elementCount++, "KneeBraceWall0");
                        sh_kneeBrace1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_kneeBrace1);

                        //Store nodes
                        //nodeLst.AddRange(knodes1);

                        //check for line intersetion and divide the curves corresponding curves
                        List<SH_Element> transBeamLst = transBeam.ToList();
                        foreach (SH_Element t in transBeamLst)
                        {
                            Point3d p1 = t.Nodes[0].Position;
                            Point3d p2 = t.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2); // t-element to check
                            double intersectPt1;
                            double intersectPt2; // parameter on checkLine that intersects with kbrace1

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace1, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            if (intersection == true)
                            {
                                Point3d intersectPt = checkLine.PointAt(intersectPt2);
                                Line brace1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes11 = new SH_Node[2];
                                knodes11[0] = new SH_Node(brace1.From, null);
                                knodes11[1] = new SH_Node(brace1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes11, _ss.elementCount++, "transversalBeam");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line brace2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes12 = new SH_Node[2];
                                knodes12[0] = new SH_Node(brace2.From, null);
                                knodes12[1] = new SH_Node(brace2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes12, _ss.elementCount++, "transversalBeam");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                // Add intersection point to nodeLst
                                SH_Node[] intersectPoint = new SH_Node[1];
                                intersectPoint[0] = new SH_Node(intersectPt, null);
                                nodeLst.AddRange(intersectPoint);

                                //Stor the SH_element that intersect, later removed
                                IDLst.Add(t.ID);
                                nameLst.Add(t.elementName);
                                //int? id = t.ID;
                                //string name = t.elementName;
                                //_ss.Elements["Line"].RemoveAll(el => el.ID == id && el.elementName == name);
                            }
                        }

                        // check intersection with column, and split
                        foreach (SH_Element c in columns.ToList())
                        {
                            Point3d p1 = c.Nodes[0].Position;
                            Point3d p2 = c.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2); //column to check
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace1, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            Point3d intersectPt = checkLine.PointAt(intersectPt2);
                            if (intersection == true && (nodeLst.Any(x => x.Position == intersectPt) == false))
                            {
                                Line c1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes11 = new SH_Node[2];
                                knodes11[0] = new SH_Node(c1.From, null);
                                knodes11[1] = new SH_Node(c1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes11, _ss.elementCount++, "topPartColumn");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line c2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes12 = new SH_Node[2];
                                knodes12[0] = new SH_Node(c2.From, null);
                                knodes12[1] = new SH_Node(c2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes12, _ss.elementCount++, "bottomPartColumn");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                // Add intersection point to nodeLst
                                SH_Node[] intersectPoint = new SH_Node[1];
                                intersectPoint[0] = new SH_Node(intersectPt, null);
                                nodeLst.AddRange(intersectPoint);

                                //Stor the SH_element that intersect, later removed
                                IDLst.Add(c.ID);
                                nameLst.Add(c.elementName);
                            }
                        }

                        //Second knee brace
                        Point3d pt2 = cols.ElementAt(0).Nodes[1].Position;
                        Point3d pt21 = new Point3d(pt2.X - distBrace, pt2.Y, pt2.Z);
                        Point3d pt22 = new Point3d(pt2.X, pt2.Y, pt2.Z - distBrace);

                        Line kbrace2 = new Line(pt21, pt22);
                        SH_Node[] knodes2 = new SH_Node[2];
                        knodes2[0] = new SH_Node(kbrace2.From, null);
                        knodes2[1] = new SH_Node(kbrace2.To, null);
                        SH_Line sh_kneeBrace2 = new SH_Line(knodes2, _ss.elementCount++, "KneeBraceWall0");
                        sh_kneeBrace2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_kneeBrace2);

                        //Store nodes
                        //nodeLst.AddRange(knodes2);

                        //check for line intersetion and diivide the curves corresponding curves
                        foreach (SH_Element t in transBeam.ToList())
                        {
                            Point3d p1 = t.Nodes[0].Position;
                            Point3d p2 = t.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2);
                            //Curve checkCurve = checkLine.ToNurbsCurve();
                            //Curve kneebrace2 = kbrace2.ToNurbsCurve();
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace2, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            if (intersection == true)
                            {
                                Point3d intersectPt = checkLine.PointAt(intersectPt2);
                                Line brace1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes21 = new SH_Node[2];
                                knodes21[0] = new SH_Node(brace1.From, null);
                                knodes21[1] = new SH_Node(brace1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes21, _ss.elementCount++, "transversalBeam");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line brace2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes22 = new SH_Node[2];
                                knodes22[0] = new SH_Node(brace2.From, null);
                                knodes22[1] = new SH_Node(brace2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes22, _ss.elementCount++, "transversalBeam");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                // Add intersection point to nodeLst
                                SH_Node[] intersectPoint = new SH_Node[1];
                                intersectPoint[0] = new SH_Node(intersectPt, null);
                                nodeLst.AddRange(intersectPoint);

                                //Stor the SH_element that intersect, later removed
                                IDLst.Add(t.ID);
                                nameLst.Add(t.elementName);
                            }
                        }

                        // check intersection with column, and split
                        foreach (SH_Element c in columns.ToList())
                        {
                            Point3d p1 = c.Nodes[0].Position;
                            Point3d p2 = c.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2);
                            //Curve checkCurve = checkLine.ToNurbsCurve();
                            //Curve kneebrace1 = kbrace1.ToNurbsCurve();
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace2, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            Point3d intersectPt = checkLine.PointAt(intersectPt2);
                            if (intersection == true && (nodeLst.Any(x => x.Position == intersectPt) == false))
                            {
                                Line c1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes21 = new SH_Node[2];
                                knodes21[0] = new SH_Node(c1.From, null);
                                knodes21[1] = new SH_Node(c1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes21, _ss.elementCount++, "topPartColumn");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line c2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes22 = new SH_Node[2];
                                knodes22[0] = new SH_Node(c2.From, null);
                                knodes22[1] = new SH_Node(c2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes22, _ss.elementCount++, "bottomPartColumn");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                // Add intersection point to nodeLst
                                SH_Node[] intersectPoint = new SH_Node[1];
                                intersectPoint[0] = new SH_Node(intersectPt, null);
                                nodeLst.AddRange(intersectPoint);

                                //Stor the SH_element that intersect, later removed
                                IDLst.Add(c.ID);
                                nameLst.Add(c.elementName);
                            }
                        }
                    }
                    //Wall 1
                    if (nrWall.ElementAt(numberLat) == 1)
                    {
                        //First knee brace
                        Point3d pt1 = cols.ElementAt(1).Nodes[1].Position;
                        Point3d pt11 = new Point3d(pt1.X + distBrace, pt1.Y, pt1.Z);
                        Point3d pt12 = new Point3d(pt1.X, pt1.Y, pt1.Z - distBrace);

                        Line kbrace1 = new Line(pt11, pt12);
                        SH_Node[] knodes1 = new SH_Node[2];
                        knodes1[0] = new SH_Node(kbrace1.From, null);
                        knodes1[1] = new SH_Node(kbrace1.To, null);
                        SH_Line sh_kneeBrace1 = new SH_Line(knodes1, _ss.elementCount++, "KneeBraceWall1");
                        sh_kneeBrace1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_kneeBrace1);

                        //Store nodes
                        //nodeLst.AddRange(knodes1);

                        //check for line intersetion and diivide the curves corresponding curves
                        foreach (SH_Element t in transBeam.ToList())
                        {
                            Point3d p1 = t.Nodes[0].Position;
                            Point3d p2 = t.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2);
                            //Curve checkCurve = checkLine.ToNurbsCurve();
                            //Curve kneebrace1 = kbrace1.ToNurbsCurve();
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace1, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            if (intersection == true)
                            {
                                Point3d intersectPt = checkLine.PointAt(intersectPt2);
                                Line brace1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes11 = new SH_Node[2];
                                knodes11[0] = new SH_Node(brace1.From, null);
                                knodes11[1] = new SH_Node(brace1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes11, _ss.elementCount++, "transversalBeam");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line brace2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes12 = new SH_Node[2];
                                knodes12[0] = new SH_Node(brace2.From, null);
                                knodes12[1] = new SH_Node(brace2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes12, _ss.elementCount++, "transversalBeam");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                //Stor the SH_element that intersect, later removed
                                IDLst.Add(t.ID);
                                nameLst.Add(t.elementName);
                            }
                        }

                        // check intersection with column, and split + make the  new column and beam
                        foreach (SH_Element c in columns.ToList())
                        {
                            Point3d p1 = c.Nodes[0].Position;
                            Point3d p2 = c.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2);
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace1, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            Point3d intersectPt = checkLine.PointAt(intersectPt2);
                            if (intersection == true && (nodeLst.Any(x => x.Position == intersectPt) == false))
                            {
                                Line c1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes11 = new SH_Node[2];
                                knodes11[0] = new SH_Node(c1.From, null);
                                knodes11[1] = new SH_Node(c1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes11, _ss.elementCount++, "topPartColumn");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line c2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes12 = new SH_Node[2];
                                knodes12[0] = new SH_Node(c2.From, null);
                                knodes12[1] = new SH_Node(c2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes12, _ss.elementCount++, "bottomPartColumn");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                // Add intersection point to nodeLst
                                SH_Node[] intersectPoint = new SH_Node[1];
                                intersectPoint[0] = new SH_Node(intersectPt, null);
                                nodeLst.AddRange(intersectPoint);

                                //Store the SH_element that intersect, later removed
                                IDLst.Add(c.ID);
                                nameLst.Add(c.elementName);
                            }
                        }

                        //Second knee brace
                        Point3d pt2 = col2.Nodes[1].Position;
                        Point3d pt21 = new Point3d(pt2.X - distBrace, pt2.Y, pt2.Z);
                        Point3d pt22 = new Point3d(pt2.X, pt2.Y, pt2.Z - distBrace);

                        Line kbrace2 = new Line(pt21, pt22);
                        SH_Node[] knodes2 = new SH_Node[2];
                        knodes2[0] = new SH_Node(kbrace2.From, null);
                        knodes2[1] = new SH_Node(kbrace2.To, null);
                        SH_Line sh_kneeBrace2 = new SH_Line(knodes2, _ss.elementCount++, "KneeBraceWall1");
                        sh_kneeBrace2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element  
                        _ss.Elements["Line"].Add(sh_kneeBrace2);

                        //Store nodes
                        //nodeLst.AddRange(knodes2);

                        //check for line intersetion and diivide the curves corresponding curves
                        foreach (SH_Element t in transBeam.ToList())
                        {
                            Point3d p1 = t.Nodes[0].Position;
                            Point3d p2 = t.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2);
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace2, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            if (intersection == true)
                            {
                                Point3d intersectPt = checkLine.PointAt(intersectPt2);
                                Line brace1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes21 = new SH_Node[2];
                                knodes21[0] = new SH_Node(brace1.From, null);
                                knodes21[1] = new SH_Node(brace1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes21, _ss.elementCount++, "transversalBeam");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line brace2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes22 = new SH_Node[2];
                                knodes22[0] = new SH_Node(brace2.From, null);
                                knodes22[1] = new SH_Node(brace2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes22, _ss.elementCount++, "transversalBeam");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                //Stor the SH_element that intersect, later removed
                                IDLst.Add(t.ID);
                                nameLst.Add(t.elementName);
                            }
                        }

                        // check intersection with column, and split
                        foreach (SH_Element c in columns.ToList())
                        {
                            Point3d p1 = c.Nodes[0].Position;
                            Point3d p2 = c.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2);
                            //Curve checkCurve = checkLine.ToNurbsCurve();
                            //Curve kneebrace1 = kbrace1.ToNurbsCurve();
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace2, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            Point3d intersectPt = checkLine.PointAt(intersectPt2);
                            if (intersection == true && (nodeLst.Any(x => x.Position == intersectPt) == false))
                            {
                                Line c1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes21 = new SH_Node[2];
                                knodes21[0] = new SH_Node(c1.From, null);
                                knodes21[1] = new SH_Node(c1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes21, _ss.elementCount++, "topPartColumn");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line c2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes22 = new SH_Node[2];
                                knodes22[0] = new SH_Node(c2.From, null);
                                knodes22[1] = new SH_Node(c2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes22, _ss.elementCount++, "bottomPartColumn");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                // Add intersection point to nodeLst
                                SH_Node[] intersectPoint = new SH_Node[1];
                                intersectPoint[0] = new SH_Node(intersectPt, null);
                                nodeLst.AddRange(intersectPoint);

                                //Stor the SH_element that intersect, later removed
                                IDLst.Add(c.ID);
                                nameLst.Add(c.elementName);
                            }
                        }
                    }

                    // Wall 2
                    if (nrWall.ElementAt(numberLat) == 2)
                    {
                        //First knee brace
                        Point3d pt1 = col1.Nodes[1].Position;
                        Point3d pt11 = new Point3d(pt1.X, pt1.Y + distBrace, pt1.Z);
                        Point3d pt12 = new Point3d(pt1.X, pt1.Y, pt1.Z - distBrace);

                        Line kbrace1 = new Line(pt11, pt12);
                        SH_Node[] knodes1 = new SH_Node[2];
                        knodes1[0] = new SH_Node(kbrace1.From, null);
                        knodes1[1] = new SH_Node(kbrace1.To, null);
                        SH_Line sh_kneeBrace1 = new SH_Line(knodes1, _ss.elementCount++, "KneeBraceWall2");
                        sh_kneeBrace1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_kneeBrace1);

                        //Store nodes
                        //nodeLst.AddRange(knodes1);

                        //check for line intersetion and divide the curves corresponding curves
                        foreach (SH_Element t in longBeam.ToList())
                        {
                            Point3d p1 = t.Nodes[0].Position;
                            Point3d p2 = t.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2);
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace1, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            if (intersection == true)
                            {
                                Point3d intersectPt = checkLine.PointAt(intersectPt2);
                                Line brace1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes11 = new SH_Node[2];
                                knodes11[0] = new SH_Node(brace1.From, null);
                                knodes11[1] = new SH_Node(brace1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes11, _ss.elementCount++, "longitudinalBeam");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line brace2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes12 = new SH_Node[2];
                                knodes12[0] = new SH_Node(brace2.From, null);
                                knodes12[1] = new SH_Node(brace2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes12, _ss.elementCount++, "longitudinalBeam");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                //Store the SH_element that intersect, later removed
                                IDLst.Add(t.ID);
                                nameLst.Add(t.elementName);
                            }
                        }

                        // check intersection with column, and split
                        foreach (SH_Element c in columns.ToList())
                        {
                            Point3d p1 = c.Nodes[0].Position;
                            Point3d p2 = c.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2);
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace1, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            Point3d intersectPt = checkLine.PointAt(intersectPt2);
                            if (intersection == true && (nodeLst.Any(x => x.Position == intersectPt) == false))
                            {
                                Line c1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes11 = new SH_Node[2];
                                knodes11[0] = new SH_Node(c1.From, null);
                                knodes11[1] = new SH_Node(c1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes11, _ss.elementCount++, "topPartColumn");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line c2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes12 = new SH_Node[2];
                                knodes12[0] = new SH_Node(c2.From, null);
                                knodes12[1] = new SH_Node(c2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes12, _ss.elementCount++, "bottomPartColumn");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                // Add intersection point to nodeLst
                                SH_Node[] intersectPoint = new SH_Node[1];
                                intersectPoint[0] = new SH_Node(intersectPt, null);
                                nodeLst.AddRange(intersectPoint);

                                //Store the SH_element that intersect, later removed
                                IDLst.Add(c.ID);
                                nameLst.Add(c.elementName);
                            }
                        }

                        //Second knee brace
                        Point3d pt2 = cols.ElementAt(1).Nodes[1].Position;
                        Point3d pt21 = new Point3d(pt2.X, pt2.Y - distBrace, pt2.Z);
                        Point3d pt22 = new Point3d(pt2.X, pt2.Y, pt2.Z - distBrace);

                        Line kbrace2 = new Line(pt21, pt22);
                        SH_Node[] knodes2 = new SH_Node[2];
                        knodes2[0] = new SH_Node(kbrace2.From, null);
                        knodes2[1] = new SH_Node(kbrace2.To, null);
                        SH_Line sh_kneeBrace2 = new SH_Line(knodes2, _ss.elementCount++, "KneeBraceWall2");
                        sh_kneeBrace2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_kneeBrace2);

                        //Store nodes
                        //nodeLst.AddRange(knodes2);

                        //check for line intersetion and diivide the curves corresponding curves
                        foreach (SH_Element t in longBeam.ToList())
                        {
                            Point3d p1 = t.Nodes[0].Position;
                            Point3d p2 = t.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2);
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace2, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            if (intersection == true)
                            {
                                Point3d intersectPt = checkLine.PointAt(intersectPt2);
                                Line brace1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes21 = new SH_Node[2];
                                knodes21[0] = new SH_Node(brace1.From, null);
                                knodes21[1] = new SH_Node(brace1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes21, _ss.elementCount++, "longitudinalBeam");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line brace2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes22 = new SH_Node[2];
                                knodes22[0] = new SH_Node(brace2.From, null);
                                knodes22[1] = new SH_Node(brace2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes22, _ss.elementCount++, "longitudinalBeam");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                //Stor the SH_element that intersect, later removed
                                IDLst.Add(t.ID);
                                nameLst.Add(t.elementName);
                            }
                        }

                        // check intersection with column, and split
                        foreach (SH_Element c in columns.ToList())
                        {
                            Point3d p1 = c.Nodes[0].Position;
                            Point3d p2 = c.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2);
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace2, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            Point3d intersectPt = checkLine.PointAt(intersectPt2);
                            if (intersection == true && (nodeLst.Any(x => x.Position == intersectPt) == false))
                            {
                                Line c1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes21 = new SH_Node[2];
                                knodes21[0] = new SH_Node(c1.From, null);
                                knodes21[1] = new SH_Node(c1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes21, _ss.elementCount++, "topPartColumn");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line c2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes22 = new SH_Node[2];
                                knodes22[0] = new SH_Node(c2.From, null);
                                knodes22[1] = new SH_Node(c2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes22, _ss.elementCount++, "bottomPartColumn");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                // Add intersection point to nodeLst
                                SH_Node[] intersectPoint = new SH_Node[1];
                                intersectPoint[0] = new SH_Node(intersectPt, null);
                                nodeLst.AddRange(intersectPoint);

                                //Stor the SH_element that intersect, later removed
                                IDLst.Add(c.ID);
                                nameLst.Add(c.elementName);
                            }
                        }
                    }

                    //Wall 3
                    if (nrWall.ElementAt(numberLat) == 3)
                    {
                        //First knee brace
                        Point3d pt1 = cols.ElementAt(0).Nodes[1].Position;
                        Point3d pt11 = new Point3d(pt1.X, pt1.Y + distBrace, pt1.Z);
                        Point3d pt12 = new Point3d(pt1.X, pt1.Y, pt1.Z - distBrace);

                        Line kbrace1 = new Line(pt11, pt12);
                        SH_Node[] knodes1 = new SH_Node[2];
                        knodes1[0] = new SH_Node(kbrace1.From, null);
                        knodes1[1] = new SH_Node(kbrace1.To, null);
                        SH_Line sh_kneeBrace1 = new SH_Line(knodes1, _ss.elementCount++, "KneeBraceWall3");
                        sh_kneeBrace1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_kneeBrace1);

                        //Store nodes
                        //nodeLst.AddRange(knodes1);

                        //check for line intersetion and diivide the curves corresponding curves
                        foreach (SH_Element t in transBeam.ToList())
                        {
                            Point3d p1 = t.Nodes[0].Position;
                            Point3d p2 = t.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2);
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace1, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            if (intersection == true)
                            {
                                Point3d intersectPt = checkLine.PointAt(intersectPt2);
                                Line brace1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes11 = new SH_Node[2];
                                knodes11[0] = new SH_Node(brace1.From, null);
                                knodes11[1] = new SH_Node(brace1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes11, _ss.elementCount++, "longitudinalBeam");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line brace2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes12 = new SH_Node[2];
                                knodes12[0] = new SH_Node(brace2.From, null);
                                knodes12[1] = new SH_Node(brace2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes12, _ss.elementCount++, "longitudinalBeam");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                //Stor the SH_element that intersect, later removed
                                IDLst.Add(t.ID);
                                nameLst.Add(t.elementName);
                            }
                        }

                        // check intersection with column, and split + construct new column and beam
                        foreach (SH_Element c in columns.ToList())
                        {
                            Point3d p1 = c.Nodes[0].Position;
                            Point3d p2 = c.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2);
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace1, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            Point3d intersectPt = checkLine.PointAt(intersectPt2);
                            if (intersection == true && (nodeLst.Any(x => x.Position == intersectPt) == false))
                            {
                                Line c1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes11 = new SH_Node[2];
                                knodes11[0] = new SH_Node(c1.From, null);
                                knodes11[1] = new SH_Node(c1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes11, _ss.elementCount++, "topPartColumn");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line c2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes12 = new SH_Node[2];
                                knodes12[0] = new SH_Node(c2.From, null);
                                knodes12[1] = new SH_Node(c2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes12, _ss.elementCount++, "bottomPartColumn");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                // Add intersection point to nodeLst
                                SH_Node[] intersectPoint = new SH_Node[1];
                                intersectPoint[0] = new SH_Node(intersectPt, null);
                                nodeLst.AddRange(intersectPoint);

                                //Stor the SH_element that intersect, later removed
                                IDLst.Add(c.ID);
                                nameLst.Add(c.elementName);
                            }
                        }

                        //Second knee brace
                        Point3d pt2 = col2.Nodes[1].Position;
                        Point3d pt21 = new Point3d(pt2.X, pt2.Y - distBrace, pt2.Z);
                        Point3d pt22 = new Point3d(pt2.X, pt2.Y, pt2.Z - distBrace);

                        Line kbrace2 = new Line(pt21, pt22);
                        SH_Node[] knodes2 = new SH_Node[2];
                        knodes2[0] = new SH_Node(kbrace2.From, null);
                        knodes2[1] = new SH_Node(kbrace2.To, null);
                        SH_Line sh_kneeBrace2 = new SH_Line(knodes2, _ss.elementCount++, "KneeBraceWall3");
                        sh_kneeBrace2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                        _ss.Elements["Line"].Add(sh_kneeBrace2);

                        //Store nodes
                        //nodeLst.AddRange(knodes2);

                        //check for line intersetion and diivide the curves corresponding curves
                        foreach (SH_Element t in transBeam.ToList())
                        {
                            Point3d p1 = t.Nodes[0].Position;
                            Point3d p2 = t.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2);
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace2, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            if (intersection == true)
                            {
                                Point3d intersectPt = checkLine.PointAt(intersectPt2); //construct point at intersetion 
                                Line brace1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes21 = new SH_Node[2];
                                knodes21[0] = new SH_Node(brace1.From, null);
                                knodes21[1] = new SH_Node(brace1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes21, _ss.elementCount++, "longitudinalBeam");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line brace2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes22 = new SH_Node[2];
                                knodes22[0] = new SH_Node(brace2.From, null);
                                knodes22[1] = new SH_Node(brace2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes22, _ss.elementCount++, "longitudinalBeam");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                //Stor the SH_element that intersect, later removed
                                IDLst.Add(t.ID);
                                nameLst.Add(t.elementName);
                            }
                        }

                        // check intersection with column, and split + construct new column and beam
                        foreach (SH_Element c in columns.ToList())
                        {
                            Point3d p1 = c.Nodes[0].Position;
                            Point3d p2 = c.Nodes[1].Position;
                            Line checkLine = new Line(p1, p2);
                            double intersectPt1;
                            double intersectPt2;

                            bool intersection = Rhino.Geometry.Intersect.Intersection.LineLine(kbrace2, checkLine, out intersectPt1, out intersectPt2, 0.001, true);
                            Point3d intersectPt = checkLine.PointAt(intersectPt2);

                            if (intersection == true && (nodeLst.Any(x => x.Position == intersectPt) == false))
                            {
                                Line c1 = new Line(checkLine.From, intersectPt);
                                SH_Node[] knodes21 = new SH_Node[2];
                                knodes21[0] = new SH_Node(c1.From, null);
                                knodes21[1] = new SH_Node(c1.To, null);
                                SH_Line sh_kneeBrace = new SH_Line(knodes21, _ss.elementCount++, "topPartColumn");
                                sh_kneeBrace.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace);

                                Line c2 = new Line(intersectPt, checkLine.To);
                                SH_Node[] knodes22 = new SH_Node[2];
                                knodes22[0] = new SH_Node(c2.From, null);
                                knodes22[1] = new SH_Node(c2.To, null);
                                SH_Line sh_kneeBrace0 = new SH_Line(knodes22, _ss.elementCount++, "bottomPartColumn");
                                sh_kneeBrace0.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_kneeBrace0);

                                // Add intersection point to nodeLst
                                SH_Node[] intersectPoint = new SH_Node[1];
                                intersectPoint[0] = new SH_Node(intersectPt, null);
                                nodeLst.AddRange(intersectPoint);

                                //Stor the SH_element that intersect, later removed
                                IDLst.Add(c.ID);
                                nameLst.Add(c.elementName);
                            }
                        }
                    }
                }

                // ------------- nrLateralStability = 2 (shear wall) ----------
                if (nrLateralStability.ElementAt(numberLat) == 3)
                {
                    if (nrWall.ElementAt(numberLat) == 0)
                    {
                        Point3d corner1 = col1.Nodes[0].Position;
                        Point3d corner2 = col1.Nodes[1].Position;
                        Point3d corner3 = cols.ElementAt(0).Nodes[1].Position;
                        Point3d corner4 = cols.ElementAt(0).Nodes[0].Position;
                        Surface srf = NurbsSurface.CreateFromCorners(corner1, corner2, corner3, corner4, 0.0001);
                        SH_Surface sh_srf = new SH_Surface(srf, "ShearWall0");
                        _ss.Elements["Surface"].Add(sh_srf);
                    }

                    if (nrWall.ElementAt(numberLat) == 1)
                    {
                        Point3d corner1 = col2.Nodes[0].Position;
                        Point3d corner2 = col2.Nodes[1].Position;
                        Point3d corner3 = cols.ElementAt(1).Nodes[1].Position;
                        Point3d corner4 = cols.ElementAt(1).Nodes[0].Position;
                        Surface srf = NurbsSurface.CreateFromCorners(corner1, corner2, corner3, corner4, 0.0001);
                        SH_Surface sh_srf = new SH_Surface(srf, "ShearWall1");
                        _ss.Elements["Surface"].Add(sh_srf);
                    }

                    if (nrWall.ElementAt(numberLat) == 2)
                    {
                        Point3d corner1 = col1.Nodes[0].Position;
                        Point3d corner2 = col1.Nodes[1].Position;
                        Point3d corner3 = cols.ElementAt(1).Nodes[1].Position;
                        Point3d corner4 = cols.ElementAt(1).Nodes[0].Position;
                        Surface srf = NurbsSurface.CreateFromCorners(corner1, corner2, corner3, corner4, 0.0001);
                        SH_Surface sh_srf = new SH_Surface(srf, "ShearWall2");
                        _ss.Elements["Surface"].Add(sh_srf);
                    }

                    if (nrWall.ElementAt(numberLat) == 3)
                    {
                        Point3d corner1 = col2.Nodes[0].Position;
                        Point3d corner2 = col2.Nodes[1].Position;
                        Point3d corner3 = cols.ElementAt(0).Nodes[1].Position;
                        Point3d corner4 = cols.ElementAt(0).Nodes[0].Position;
                        Surface srf = NurbsSurface.CreateFromCorners(corner1, corner2, corner3, corner4, 0.0001);
                        SH_Surface sh_srf = new SH_Surface(srf, "ShearWall3");
                        _ss.Elements["Surface"].Add(sh_srf);
                    }
                }

                // -------------nrLateralStability = 2(none)----------
                if (nrLateralStability.ElementAt(numberLat) == 4 && beamMat.Name == "concrete")
                {
                    if (nrWall.ElementAt(numberLat) == 0 || nrWall.ElementAt(numberLat) == 1 || nrWall.ElementAt(numberLat) == 2 || nrWall.ElementAt(numberLat) == 3)
                    {
                    }
                }
            }
                //Add nodeFs to Simple Shape
                //_ss.Nodes = new List<SH_Node>();
                _ss.Nodes.AddRange(nodeLst);

            //Remove the SH_element that intersect
            for (int r = 0; r < IDLst.Count; r++)
            {
                int? id = IDLst.ElementAt(r);
                string name = nameLst.ElementAt(r);
                _ss.Elements["Line"].RemoveAll(el => el.ID == id && el.elementName == name);
            }

            // change the state
            _ss.SimpleShapeState = State.zeta;
            return "LateralStability successfully applied.";

        }


        public override State GetNextState()
        {
            return State.epsilon;
        }

    }
}
