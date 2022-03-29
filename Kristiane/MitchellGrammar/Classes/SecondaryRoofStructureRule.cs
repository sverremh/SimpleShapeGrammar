﻿using System;
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
    public class SecondaryRoofStructureRule : SH_Rule
    {
        // --- properties ---
        public int nrSecondaryRS;
        public double h;
        public int count;
        public int numberSecondaryRS;

        // --- constructors ---
        public SecondaryRoofStructureRule()
        {
            Name = "SecondaryRoofStructureClass";
            RuleState = State.delta;
        }

        public SecondaryRoofStructureRule(int _nrSecondaryRS, double _h, int _count, int _numberSecondaryRS)
        {
            nrSecondaryRS = _nrSecondaryRS;
            h = _h;
            count = _count;
            numberSecondaryRS = _numberSecondaryRS;
            Name = "SecondaryRoofStructureClass";
            RuleState = State.delta;
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
                return "The State is not compatible with PrimaryRoofStructureRule.";
            }
            
      
            // Get the substructure number from MRule2 and primary substructure MRule3
            string name = _ss.name;
            string[] nummberStructure = name.Split('_');

            List<SH_Node> nodeLst = new List<SH_Node>(); //list for nodes

            // ------------- SECONDARY ROOF STRUCTURE FOR SUBSTRUCTUREE 0 AND PRIMARY ROOF STRUCTURE 0 (TRUSS) ------------- 
            if (nummberStructure[1] == "0" && nummberStructure[2] == "0")
            {
                //Collect the two elements that are the transversal beams (bottom beam and top beam)
                //Bottom beams
                var hbTBeams = from hbTBeam in _ss.Elements["Line"]
                               where hbTBeam.elementName == "hbTrussPrimarySub0"
                               select hbTBeam;

                //Top beam¨s
                var htTBeams = from htTBeam in _ss.Elements["Line"]
                               where htTBeam.elementName == "htTrussPrimarySub0"
                               select htTBeam;

                //Store necessary points to construct pitched trusses
                //Horizontal bottom points for one side
                List<Point3d> hbPts1 = new List<Point3d>(); //list for bottom points
                for (int el1 = 0; el1 < hbTBeams.Count() / 2; el1++)
                {
                    //Nodes for bottom beams
                    SH_Element hb = hbTBeams.ElementAt(el1);
                    hbPts1.Add(hb.Nodes[0].Position);
                }
                SH_Element hb1 = hbTBeams.ElementAt(hbTBeams.Count() / 2 - 1);
                hbPts1.Add(hb1.Nodes[1].Position);

                //Horizontal bttom points for another side
                List<Point3d> hbPts2 = new List<Point3d>(); //list for bottom points
                for (int el2 = hbTBeams.Count() / 2; el2 < hbTBeams.Count(); el2++)
                {
                    //Nodes for bottom beams
                    SH_Element hb = hbTBeams.ElementAt(el2);
                    hbPts2.Add(hb.Nodes[0].Position);
                }
                SH_Element hb2 = hbTBeams.Last();
                hbPts2.Add(hb2.Nodes[1].Position);

                //Horizontal top points for one side
                List<Point3d> htPts1 = new List<Point3d>(); //list for top points
                for (int el = 0; el < htTBeams.Count() / 2; el++)
                {
                    //Nodes for top beams
                    SH_Element tb = htTBeams.ElementAt(el);
                    htPts1.Add(tb.Nodes[0].Position);
                }
                SH_Element tb1 = htTBeams.ElementAt(htTBeams.Count() / 2 - 1);
                htPts1.Add(tb1.Nodes[1].Position);

                //Horizontal top points for anpther side
                List<Point3d> htPts2 = new List<Point3d>(); //list for top points
                for (int el = htTBeams.Count() / 2; el < htTBeams.Count(); el++)
                {
                    //Nodes for top beams
                    SH_Element tb = htTBeams.ElementAt(el);
                    htPts2.Add(tb.Nodes[0].Position);
                }
                SH_Element btb2 = htTBeams.Last();
                htPts2.Add(btb2.Nodes[1].Position);

                //---------------- PITCHED TRUSSES as secondary roof structure ----------------
                if (nrSecondaryRS == 0)
                {
                    for (int num = 0; num < hbPts1.Count(); num++)
                    {
                        double hbeam = hbPts1[0].DistanceTo(htPts1[0]); //Height primary structure
                        Line bl = new Line(hbPts1[num], hbPts2[num]); //bl = bottom line (bottom beam)
                        Point3d midPt = bl.PointAt(0.5);
                        Point3d tmidPt = new Point3d(midPt.X, midPt.Y, midPt.Z + hbeam + h); //highest point of secondary roof structure

                        //Bottom beam
                        Point3d[] bPts;
                        Curve bcrv = bl.ToNurbsCurve();
                        bcrv.DivideByCount(Convert.ToInt32(count), true, out bPts);

                        //Inclined beams, pitched beams
                        Point3d[] pPts;
                        List<Point3d> ppts = new List<Point3d>();
                        ppts.Add(htPts1[num]);
                        ppts.Add(tmidPt);
                        ppts.Add(htPts2[num]);
                        Curve pcrv = Curve.CreateControlPointCurve(ppts, 1);
                        pcrv.DivideByCount(Convert.ToInt32(count), true, out pPts);

                        //Horizontal bottom lines
                        for (int i = 0; i < bPts.Count() - 1; i++)
                        {
                            Line hbline = new Line(bPts[i], bPts[i + 1]);
                            SH_Node[] hbnodes = new SH_Node[2];
                            hbnodes[0] = new SH_Node(hbline.From, null);
                            hbnodes[1] = new SH_Node(hbline.To, null);
                            SH_Line sh_hbTrussSecondary = new SH_Line(hbnodes, _ss.elementCount++, "hbPitchedTruss_Mitchell_0_0"); //bottom horizontals for trusses for Secondary Roof Strucutre, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_hbTrussSecondary);
                        }

                        //Vertical lines
                        for (int n = 1; n < bPts.Count() - 1; n++)
                        {
                            Line vline = new Line(bPts[n], pPts[n]);
                            SH_Node[] vnodes = new SH_Node[2];
                            vnodes[0] = new SH_Node(vline.From, null);
                            vnodes[1] = new SH_Node(vline.To, null);
                            SH_Line sh_vTrussSecondary = new SH_Line(vnodes, _ss.elementCount++, "vPitchedTruss_Mitchell_0_0"); //verticals for trusses for Secondary Roof Strucutre, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_vTrussSecondary);
                        }

                        //Pitched lines
                        for (int j = 0; j < pPts.Count() - 1; j++)
                        {
                            Line pLine = new Line(pPts[j], pPts[j + 1]);
                            SH_Node[] pnodes = new SH_Node[2];
                            pnodes[0] = new SH_Node(pLine.From, null);
                            pnodes[1] = new SH_Node(pLine.To, null);
                            SH_Line sh_pTrussSecondary = new SH_Line(pnodes, _ss.elementCount++, "pPitchedTruss_Mitchell_0_0"); //inclined lined  for trusses for Secondary Roof Strucutre, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_pTrussSecondary);
                        }

                        //Trusses/diagonals
                        for (int k = 1; k < bPts.Count() / 2; k++)
                        {
                            Line line = new Line(pPts[k], bPts[k + 1]);
                            SH_Node[] nodes = new SH_Node[2];
                            nodes[0] = new SH_Node(line.From, null);
                            nodes[1] = new SH_Node(line.To, null);
                            SH_Line sh_dTrussSecondary = new SH_Line(nodes, _ss.elementCount++, "dPitchedTruss_Mitchell_0_0"); //diagonals for trusses for Secondary Roof Strucutre, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_dTrussSecondary);
                        }
                        for (int m = bPts.Count() / 2; m < bPts.Count() - 2; m++)
                        {
                            Line line = new Line(bPts[m], pPts[m + 1]);
                            SH_Node[] nodes = new SH_Node[2];
                            nodes[0] = new SH_Node(line.From, null);
                            nodes[1] = new SH_Node(line.To, null);
                            SH_Line sh_dTrussSecondary = new SH_Line(nodes, _ss.elementCount++, "dPitchedTruss_Mitchell_0_0"); //diagonals for trusses for Secondary Roof Strucutre, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_dTrussSecondary);
                        }

                        //store nodes from bPts and pPts
                        for (int node1 = 0; node1 < bPts.Length; node1++)
                        {
                            SH_Node[] bnodes = new SH_Node[1];
                            bnodes[0] = new SH_Node(bPts[node1], null);
                            nodeLst.Add(bnodes[0]);
                        }
                        for (int node1 = 0; node1 < pPts.Length; node1++)
                        {
                            SH_Node[] pnodes = new SH_Node[1];
                            pnodes[0] = new SH_Node(pPts[node1], null);
                            nodeLst.Add(pnodes[0]);
                        }
                    }
                }
                //---------------- BOWED TRUSSES as secondary roof structure ----------------
                if (nrSecondaryRS == 1)
                {
                    for (int num = 0; num < hbPts1.Count(); num++)
                    {
                        double hbeam = hbPts1[0].DistanceTo(htPts1[0]); //Height primary structure
                        Line bl = new Line(hbPts1[num], hbPts2[num]); //bl = bottom line (bottom beam)
                        Point3d midPt = bl.PointAt(0.5);
                        Point3d tmidPt = new Point3d(midPt.X, midPt.Y, midPt.Z + hbeam + h); //highest point of secondary roof structure

                        //Bottom beam
                        Point3d[] bPts;
                        Curve bcrv = bl.ToNurbsCurve();
                        bcrv.DivideByCount(Convert.ToInt32(count), true, out bPts);

                        //Arch
                        Point3d[] bowedPts;
                        List<Point3d> ppts = new List<Point3d>();
                        ppts.Add(htPts1[num]);
                        ppts.Add(tmidPt);
                        ppts.Add(htPts2[num]);
                        Curve pcrv = Curve.CreateControlPointCurve(ppts, 2);
                        pcrv.DivideByCount(Convert.ToInt32(count), true, out bowedPts);

                        //Horizontal bottom lines
                        for (int i = 0; i < bPts.Count() - 1; i++)
                        {
                            Line hbline = new Line(bPts[i], bPts[i + 1]);
                            SH_Node[] hbnodes = new SH_Node[2];
                            hbnodes[0] = new SH_Node(hbline.From, null);
                            hbnodes[1] = new SH_Node(hbline.To, null);
                            SH_Line sh_hbTrussSecondary = new SH_Line(hbnodes, _ss.elementCount++, "hbBowedTruss_Mitchell_0_0"); //Bottom horizontals for trusses for Secondary Roof Structure, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_hbTrussSecondary);
                        }

                        //Vertical lines
                        for (int n = 1; n < bPts.Count() - 1; n++)
                        {
                            Line vline = new Line(bPts[n], bowedPts[n]);
                            SH_Node[] vnodes = new SH_Node[2];
                            vnodes[0] = new SH_Node(vline.From, null);
                            vnodes[1] = new SH_Node(vline.To, null);
                            SH_Line sh_vTrussSecondary = new SH_Line(vnodes, _ss.elementCount++, "vBowedTruss_Mitchell_0_0"); //Verticals for trusses for Secondary Roof Structure, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_vTrussSecondary);
                        }

                        //Bowed lines
                        for (int j = 0; j < bowedPts.Count() - 1; j++)
                        {
                            Line pLine = new Line(bowedPts[j], bowedPts[j + 1]);
                            SH_Node[] pnodes = new SH_Node[2];
                            pnodes[0] = new SH_Node(pLine.From, null);
                            pnodes[1] = new SH_Node(pLine.To, null);
                            SH_Line sh_pTrussSecondary = new SH_Line(pnodes, _ss.elementCount++, "pBowedTruss_Mitchell_0_0"); //Bowed lines  for trusses for Secondary Roof Structure, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_pTrussSecondary);
                        }

                        //Trusses/diagonals
                        for (int k = 0; k < bPts.Count() / 2; k++)
                        {
                            Line line = new Line(bowedPts[k], bPts[k + 1]);
                            SH_Node[] nodes = new SH_Node[2];
                            nodes[0] = new SH_Node(line.From, null);
                            nodes[1] = new SH_Node(line.To, null);
                            SH_Line sh_dTrussSecondary = new SH_Line(nodes, _ss.elementCount++, "dBowedTruss_Mitchell_0_0"); //Diagonals for trusses for Secondary Roof Structure, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_dTrussSecondary);
                        }
                        for (int m = bPts.Count() / 2; m < bPts.Count() - 1; m++)
                        {
                            Line line = new Line(bPts[m], bowedPts[m + 1]);
                            SH_Node[] nodes = new SH_Node[2];
                            nodes[0] = new SH_Node(line.From, null);
                            nodes[1] = new SH_Node(line.To, null);
                            SH_Line sh_dTrussSecondary = new SH_Line(nodes, _ss.elementCount++, "dBowwedTruss_Mitchell_0_0"); //Diagonals for trusses for Secondary Roof Structure, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_dTrussSecondary);
                        }

                        //store nodes from bPts and pPts
                        for (int node1 = 0; node1 < bPts.Length; node1++)
                        {
                            SH_Node[] bnodes = new SH_Node[1];
                            bnodes[0] = new SH_Node(bPts[node1], null);
                            nodeLst.Add(bnodes[0]);
                        }
                        for (int node1 = 0; node1 < bowedPts.Length; node1++)
                        {
                            SH_Node[] pnodes = new SH_Node[1];
                            pnodes[0] = new SH_Node(bowedPts[node1], null);
                            nodeLst.Add(pnodes[0]);
                        }
                    }
                }

                //---------------- BEAMS as secondary roof structure ----------------
                if (nrSecondaryRS == 2)
                {
                    for (int num = 0; num < htPts1.Count(); num++)
                    {
                        Line secBeam = new Line(htPts1[num], htPts2[num]);
                        SH_Node[] nodes = new SH_Node[2];
                        nodes[0] = new SH_Node(secBeam.From, null);
                        nodes[1] = new SH_Node(secBeam.To, null);
                        SH_Line sh_BeamsSecondary = new SH_Line(nodes, _ss.elementCount++, "Beams_Mitchell_0_0"); //Horiszontal Beams for Secondary Roof Structure, substructure 0 Primary roof structure 0
                        _ss.Elements["Line"].Add(sh_BeamsSecondary);
                    }
                    //Remove transversal and pitched beams
                    _ss.Elements["Line"].RemoveAll(el => el.elementName == "longBeamSub0");
                }

                //---------------- FLAT TRUSSES as secondary roof structure ----------------
                if (nrSecondaryRS == 3)
                {
                    for (int num = 0; num < hbPts1.Count(); num++)
                    {
                        double hbeam = hbPts1[0].DistanceTo(htPts1[0]); //Height primary structure
                        Line bl = new Line(hbPts1[num], hbPts2[num]); //bl = bottom line (bottom beam)
                        
                        //Bottom beam
                        Point3d[] bPts;
                        Curve bcrv = bl.ToNurbsCurve();
                        bcrv.DivideByCount(Convert.ToInt32(count), true, out bPts);

                        //Top
                        Point3d[] tPts;
                        List<Point3d> ppts = new List<Point3d>();
                        Line l = new Line(htPts1[num], htPts2[num]);
                        Curve tcrv = l.ToNurbsCurve();
                        tcrv.DivideByCount(Convert.ToInt32(count), true, out tPts);

                        //Horizontal bottom lines
                        for (int i = 0; i < bPts.Count() - 1; i++)
                        {
                            Line hbline = new Line(bPts[i], bPts[i + 1]);
                            SH_Node[] hbnodes = new SH_Node[2];
                            hbnodes[0] = new SH_Node(hbline.From, null);
                            hbnodes[1] = new SH_Node(hbline.To, null);
                            SH_Line sh_hbTrussSecondary = new SH_Line(hbnodes, _ss.elementCount++, "hbFlatTruss_Mitchell_0_0"); //bottom horizontals for trusses for Secondary Roof Structure, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_hbTrussSecondary);
                        }

                        //Vertical lines
                        for (int n = 1; n < bPts.Count() - 1; n++)
                        {
                            Line vline = new Line(bPts[n], tPts[n]);
                            SH_Node[] vnodes = new SH_Node[2];
                            vnodes[0] = new SH_Node(vline.From, null);
                            vnodes[1] = new SH_Node(vline.To, null);
                            SH_Line sh_vTrussSecondary = new SH_Line(vnodes, _ss.elementCount++, "vFlatTruss_Mitchell_0_0"); //verticals for trusses for Secondary Roof Structure, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_vTrussSecondary);
                        }

                        //Horizontal top lines
                        for (int j = 0; j < tPts.Count() - 1; j++)
                        {
                            Line tLine = new Line(tPts[j], tPts[j + 1]);
                            SH_Node[] nodes = new SH_Node[2];
                            nodes[0] = new SH_Node(tLine.From, null);
                            nodes[1] = new SH_Node(tLine.To, null);
                            SH_Line sh_pTrussSecondary = new SH_Line(nodes, _ss.elementCount++, "pFlatTruss_Mitchell_0_0"); //top horizontals  for trusses for Secondary Roof Structure, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_pTrussSecondary);
                        }

                        //Trusses/diagonals
                        for (int k = 1; k < bPts.Count()-1; k+=2)
                        {
                            Line line = new Line(tPts[k], bPts[k + 1]);
                            SH_Node[] nodes = new SH_Node[2];
                            nodes[0] = new SH_Node(line.From, null);
                            nodes[1] = new SH_Node(line.To, null);
                            SH_Line sh_dTrussSecondary = new SH_Line(nodes, _ss.elementCount++, "dFlatTruss_Mitchell_0_0"); //diagonals for trusses for Secondary Roof Structure, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_dTrussSecondary);
                        }
                        for (int m = 0; m < bPts.Count() - 2; m+=2)
                        {
                            Line line = new Line(bPts[m], tPts[m + 1]);
                            SH_Node[] nodes = new SH_Node[2];
                            nodes[0] = new SH_Node(line.From, null);
                            nodes[1] = new SH_Node(line.To, null);
                            SH_Line sh_dTrussSecondary = new SH_Line(nodes, _ss.elementCount++, "dFlatTruss_Mitchell_0_0"); //diagonals for trusses for Secondary Roof Structure, substructure 0 Primary roof structure 0
                            _ss.Elements["Line"].Add(sh_dTrussSecondary);
                        }

                        //store nodes from bPts and pPts
                        for (int node1 = 0; node1 < bPts.Length; node1++)
                        {
                            SH_Node[] bnodes = new SH_Node[1];
                            bnodes[0] = new SH_Node(bPts[node1], null);
                            nodeLst.Add(bnodes[0]);
                        }
                        for (int node1 = 0; node1 < tPts.Length; node1++)
                        {
                            SH_Node[] pnodes = new SH_Node[1];
                            pnodes[0] = new SH_Node(tPts[node1], null);
                            nodeLst.Add(pnodes[0]);
                        }
                    }
                }
            }

            // ------------- SECONDARY ROOF STRUCTURE FOR SUBSTRUCTURE 0 AND PRIMARY ROOF STRUCTURE 1 (SOLID BEAM) -------------
            if (nummberStructure[1] == "0" && nummberStructure[2] == "1")
            {
                //Collect the two elements that are the transversal beams (bottom beam and top beam)
                //Transversal beams
                var tBeams = from tBeam in _ss.Elements["Line"]
                             where tBeam.elementName == "transBeamSub0"
                             select tBeam;

                //End points at first beam
                Point3d pt11 = tBeams.ElementAt(0).Nodes[0].Position;
                Point3d pt12 = tBeams.ElementAt(0).Nodes[1].Position;

                //End points at second beam
                Point3d pt21 = tBeams.ElementAt(1).Nodes[0].Position;
                Point3d pt22 = tBeams.ElementAt(1).Nodes[1].Position;

                //Construct transversal beams, divide them repsect to numberSecondaryRS
                Point3d[] pts1;
                Line tLine1 = new Line(pt11, pt12);
                Curve tcrv1 = tLine1.ToNurbsCurve();
                tcrv1.DivideByCount(numberSecondaryRS, true, out pts1);

                Point3d[] pts2;
                Line tLine2 = new Line(pt21, pt22);
                Curve tcrv2 = tLine2.ToNurbsCurve();
                tcrv2.DivideByCount(numberSecondaryRS, true, out pts2);

                //store nodes from pts1 and pts2, end nodes already stored
                for (int node1 = 1; node1 < pts1.Count() - 1; node1++)
                {
                    SH_Node[] nodes1 = new SH_Node[1];
                    nodes1[0] = new SH_Node(pts1[node1], null);
                    nodeLst.Add(nodes1[0]);
                }
                for (int node1 = 1; node1 < pts2.Count() - 1; node1++)
                {
                    SH_Node[] nodes2 = new SH_Node[1];
                    nodes2[0] = new SH_Node(pts2[node1], null);
                    nodeLst.Add(nodes2[0]);
                }

                //Horizontal transversal lines
                for (int i = 0; i < pts1.Count() - 1; i++)
                {
                    Line htline = new Line(pts1[i], pts1[i + 1]);
                    SH_Node[] htnodes = new SH_Node[2];
                    htnodes[0] = new SH_Node(htline.From, null);
                    htnodes[1] = new SH_Node(htline.To, null);
                    SH_Line sh_htSecondary = new SH_Line(htnodes, _ss.elementCount++, "hTransPitchedTruss_Mitchell_0_1"); //horizontal transversals for Secondary Roof Structure, substructure 0 Primary roof structure 1
                    _ss.Elements["Line"].Add(sh_htSecondary);
                }

                for (int j = 0; j < pts2.Count() - 1; j++)
                {
                    Line htline = new Line(pts2[j], pts2[j + 1]);
                    SH_Node[] htnodes = new SH_Node[2];
                    htnodes[0] = new SH_Node(htline.From, null);
                    htnodes[1] = new SH_Node(htline.To, null);
                    SH_Line sh_htSecondary = new SH_Line(htnodes, _ss.elementCount++, "hTransPitchedTruss_Mitchell_0_1"); //horizontal transversals for Secondary Roof Structure, substructure 0 Primary roof structure 1
                    _ss.Elements["Line"].Add(sh_htSecondary);
                }

                //---------------- PITCHED TRUSS as secondary roof structure ----------------
                //FØRSTE SOM SKAL TESTES
                if (nrSecondaryRS == 0)
                {
                    //Construct the pitched truss
                    for (int k = 0; k < pts1.Count(); k++)
                    {
                        // Bottom (horizontal) lines/beams for pitched truss
                        Point3d[] bpts;
                        Line bLine = new Line(pts1[k], pts2[k]);
                        Curve bcrv = bLine.ToNurbsCurve();
                        tcrv1.DivideByCount(count, true, out bpts);

                        for (int m = 0; m < bpts.Count() - 1; m++)
                        {
                            Line hline = new Line(bpts[m], bpts[m + 1]);
                            SH_Node[] hnodes = new SH_Node[2];
                            hnodes[0] = new SH_Node(hline.From, null);
                            hnodes[1] = new SH_Node(hline.To, null);
                            SH_Line sh_hbSecondary = new SH_Line(hnodes, _ss.elementCount++, "hPitchedTruss_Mitchell_0_1"); //horizontal bottom lines for Secondary Roof Structure, substructure 0 Primary roof structure 1
                            _ss.Elements["Line"].Add(sh_hbSecondary);
                        }

                        if (count % 2 == 0) //even counts
                        {
                            // Pitched lines/beams  
                            Point3d midpt = bLine.PointAt(0.5);
                            Point3d moveMidtPt = new Point3d(midpt.X, midpt.Y, midpt.Y + h);
                            List<Point3d> pPts = new List<Point3d>(); //store points to create a polyline
                            pPts.Add(bpts[k]);
                            pPts.Add(moveMidtPt);
                            pPts.Add(bpts[k + 1]);
                            Curve pcrv = Curve.CreateControlPointCurve(pPts, 1);

                            //Count secondary roof structuree
                            Point3d[] bpPts; //bottom pitched points
                            pcrv.DivideByCount(count, true, out bpPts);

                            //Vertical lines
                            for (int n = 1; n < bpts.Count() - 1; n++)
                            {
                                Line vline = new Line(bpts[n],bpPts[n]);
                                SH_Node[] vnodes = new SH_Node[2];
                                vnodes[0] = new SH_Node(vline.From, null);
                                vnodes[1] = new SH_Node(vline.To, null);
                                SH_Line sh_vTrussPrimary = new SH_Line(vnodes, _ss.elementCount++, "vPitchedTruss_Mitchell_0_1"); //verticals for trusses for Primary Roof Strucutre, substructure 0 Primary roof structure 1
                                _ss.Elements["Line"].Add(sh_vTrussPrimary);
                            }

                            //Pitched lines
                            for (int p = 0; p < bpPts.Count() - 1; p++)
                            {
                                Line pLine = new Line(bpPts[p], bpPts[p + 1]);
                                SH_Node[] pnodes = new SH_Node[2];
                                pnodes[0] = new SH_Node(pLine.From, null);
                                pnodes[1] = new SH_Node(pLine.To, null);
                                SH_Line sh_pTrussPrimary = new SH_Line(pnodes, _ss.elementCount++, "pPitchedTruss_Mitchell_0_1"); //inclined lines for trusses for Primary Roof Strucutre, substructure 0 Primary roof structure 1
                                _ss.Elements["Line"].Add(sh_pTrussPrimary);
                            }

                            //Trusses/diagonals
                            for (int d1 = 1; d1 < bpts.Count() / 2; d1++)
                            {
                                Line line = new Line(bpPts[d1], bpts[d1 + 1]);
                                SH_Node[] nodes = new SH_Node[2];
                                nodes[0] = new SH_Node(line.From, null);
                                nodes[1] = new SH_Node(line.To, null);
                                SH_Line sh_dTrussPrimary = new SH_Line(nodes, _ss.elementCount++, "dPitchedTruss_Mitchell_0_1"); //diagonals for trusses for Primary Roof Strucutre, substructure 0 Primary roof structure 1
                                _ss.Elements["Line"].Add(sh_dTrussPrimary);
                            }
                            for (int d2 = bpts.Count() / 2; d2 < bpts.Count() - 2; d2++)
                            {
                                Line line = new Line(bpts[d2], bpPts[d2 + 1]);
                                SH_Node[] nodes = new SH_Node[2];
                                nodes[0] = new SH_Node(line.From, null);
                                nodes[1] = new SH_Node(line.To, null);
                                SH_Line sh_dTrussPrimary = new SH_Line(nodes, _ss.elementCount++, "dPitchedTruss_Mitchell_0_1"); //diagonals for trusses for Primary Roof Strucutre, substructure 0 Primary roof structure 1
                                _ss.Elements["Line"].Add(sh_dTrussPrimary);
                            }

                            //store nodes from bpts and bpPts
                            for (int node1 = 0; node1 < bpts.Length; node1++)
                            {
                                SH_Node[] tnodes = new SH_Node[1];
                                tnodes[0] = new SH_Node(bpts[node1], null);
                                nodeLst.Add(tnodes[0]);
                            }
                            for (int node1 = 0; node1 < bpPts.Length; node1++)
                            {
                                SH_Node[] tnodes = new SH_Node[1];
                                tnodes[0] = new SH_Node(bpPts[node1], null);
                                nodeLst.Add(tnodes[0]);
                            }
                        }
                    }
                    //Remove transversal and pitched beams
                    _ss.Elements["Line"].RemoveAll(el => el.elementName == "longBeamSub0");
                    _ss.Elements["Line"].RemoveAll(el => el.elementName == "transBeamSub0");
                }
                /*
                //---------------- Bowed Trusses as secondary roof structure ----------------
                //IKKE TESTET
                if (nrSecondaryRS == 1)
                {
                    //Horizontal transversal lines
                    for (int i = 0; i < pts1.Count() - 1; i++)
                    {
                        Line htline = new Line(pts1[i], pts1[i + 1]);
                        SH_Node[] htnodes = new SH_Node[2];
                        htnodes[0] = new SH_Node(htline.From, null);
                        htnodes[1] = new SH_Node(htline.To, null);
                        SH_Line sh_htSecondary = new SH_Line(htnodes, _ss.elementCount++, "hTransBowedTruss_Mitchell_0_1"); //horizontal trransversals for Secondary Roof Structure, substructure 0 Primary roof structure 1
                        _ss.Elements["Line"].Add(sh_htSecondary);
                    }

                    for (int j = 0; j < pts2.Count() - 1; j++)
                    {
                        Line htline = new Line(pts2[j], pts2[j + 1]);
                        SH_Node[] htnodes = new SH_Node[2];
                        htnodes[0] = new SH_Node(htline.From, null);
                        htnodes[1] = new SH_Node(htline.To, null);
                        SH_Line sh_htSecondary = new SH_Line(htnodes, _ss.elementCount++, "hTransBowedTruss_Mitchell_0_1"); //horizontal transversals for Secondary Roof Structure, substructure 0 Primary roof structure 1
                        _ss.Elements["Line"].Add(sh_htSecondary);
                    }

                    //Construct the pitched truss
                    for (int k = 0; k < pts1.Length; k++)
                    {
                        // Bottom (horizontal) lines/beams for pitched truss
                        Point3d[] bpts;
                        Line bLine = new Line(pts1[k], pts2[k]);
                        Curve bcrv = bLine.ToNurbsCurve();
                        tcrv1.DivideByCount(count, true, out bpts);

                        for (int m = 0; m < bpts.Length - 1; m++)
                        {
                            Line hline = new Line(bpts[m], bpts[m + 1]);
                            SH_Node[] hnodes = new SH_Node[2];
                            hnodes[0] = new SH_Node(hline.From, null);
                            hnodes[1] = new SH_Node(hline.To, null);
                            SH_Line sh_hbSecondary = new SH_Line(hnodes, _ss.elementCount++, "hbBowedTruss_Mitchell_0_1"); //horizontal bottom lines for Secondary Roof Structure, substructure 0 Primary roof structure 1
                            _ss.Elements["Line"].Add(sh_hbSecondary);
                        }

                        // Bowed lines/beams  
                        Point3d midpt = bLine.PointAt(0.5);
                        Point3d moveMidtPt = new Point3d(midpt.X, midpt.Y, midpt.Y + h);
                        List<Point3d> pPts = new List<Point3d>(); //store points to create a polyline
                        pPts.Add(bpts[k]);
                        pPts.Add(moveMidtPt);
                        pPts.Add(bpts[k + 1]);
                        Curve pcrv = Curve.CreateControlPointCurve(pPts, 2);

                        //Count secondary roof structuree
                        Point3d[] bpPts; //bottom pitched points
                        pcrv.DivideByCount(count, true, out bpPts);

                        //Vertical lines
                        for (int n = 1; n < bpts.Count() - 1; n++)
                        {
                            Line vline = new Line(bpts[n], bpPts[n]);
                            SH_Node[] vnodes = new SH_Node[2];
                            vnodes[0] = new SH_Node(vline.From, null);
                            vnodes[1] = new SH_Node(vline.To, null);
                            SH_Line sh_vTrussPrimary = new SH_Line(vnodes, _ss.elementCount++, "vBowedTruss_Mitchell_0_1"); //verticals for trusses for Primary Roof Strucutre, substructure 0 Primary roof structure 1
                            _ss.Elements["Line"].Add(sh_vTrussPrimary);
                        }

                        //Pitched lines
                        for (int p = 0; p < bpPts.Count() - 1; p++)
                        {
                            Line pLine = new Line(bpPts[p], bpPts[p + 1]);
                            SH_Node[] pnodes = new SH_Node[2];
                            pnodes[0] = new SH_Node(pLine.From, null);
                            pnodes[1] = new SH_Node(pLine.To, null);
                            SH_Line sh_pTrussPrimary = new SH_Line(pnodes, _ss.elementCount++, "pBowedTruss_Mitchell_0_1"); //inclined lines for trusses for Primary Roof Strucutre, substructure 0 Primary roof structure 1
                            _ss.Elements["Line"].Add(sh_pTrussPrimary);
                        }

                        //Trusses/diagonals
                        for (int d1 = 1; d1 < bpts.Count() / 2; d1++)
                        {
                            Line line = new Line(bpPts[d1], bpts[d1 + 1]);
                            SH_Node[] nodes = new SH_Node[2];
                            nodes[0] = new SH_Node(line.From, null);
                            nodes[1] = new SH_Node(line.To, null);
                            SH_Line sh_dTrussPrimary = new SH_Line(nodes, _ss.elementCount++, "dBowedTruss_Mitchell_0_1"); //diagonals for trusses for Primary Roof Strucutre, substructure 0 Primary roof structure 1
                            _ss.Elements["Line"].Add(sh_dTrussPrimary);
                        }
                        for (int d2 = bpts.Count() / 2; d2 < bpts.Count() - 2; d2++)
                        {
                            Line line = new Line(bpts[d2], bpPts[d2 + 1]);
                            SH_Node[] nodes = new SH_Node[2];
                            nodes[0] = new SH_Node(line.From, null);
                            nodes[1] = new SH_Node(line.To, null);
                            SH_Line sh_dTrussPrimary = new SH_Line(nodes, _ss.elementCount++, "dBowedTruss_Mitchell_0_1"); //diagonals for trusses for Primary Roof Strucutre, substructure 0 Primary roof structure 1
                            _ss.Elements["Line"].Add(sh_dTrussPrimary);
                        }

                        //store nodes from bpts and bpPts
                        for (int node1 = 0; node1 < bpts.Length; node1++)
                        {
                            SH_Node[] tnodes = new SH_Node[1];
                            tnodes[0] = new SH_Node(bpts[node1], null);
                            nodeLst.Add(tnodes[0]);
                        }
                        for (int node1 = 0; node1 < bpPts.Length; node1++)
                        {
                            SH_Node[] tnodes = new SH_Node[1];
                            tnodes[0] = new SH_Node(bpPts[node1], null);
                            nodeLst.Add(tnodes[0]);
                        }

                    }
                }
                */

                //---------------- BEAMS as secondary roof structure ----------------
                //OK TESTET
                if (nrSecondaryRS == 2)
                {
                    //Construct the secondary beams
                    for (int k = 0; k < pts1.Length; k++)
                    {
                        Line bLine = new Line(pts1[k], pts2[k]);           
                        SH_Node[] hnodes = new SH_Node[2];
                        hnodes[0] = new SH_Node(bLine.From, null);
                        hnodes[1] = new SH_Node(bLine.To, null);
                        SH_Line sh_hbSecondary = new SH_Line(hnodes, _ss.elementCount++, "hBeams_Mitchell_0_1"); //horizontal bottom lines for Secondary Roof Structure, substructure 0 Primary roof structure 1
                        _ss.Elements["Line"].Add(sh_hbSecondary);                         
                    }
                }
                
                
                //---------------- FLAT TRUSSES as secondary roof structure ----------------  
                //IKKE TESTET
                if (nrSecondaryRS == 3)
                { 
                    //Construct the Flat truss
                    for (int num = 0; num < pts1.Count(); num++)
                    {
                        // Bottom (horizontal) lines/beams for flat truss
                        Point3d[] bpts;
                        Line bLine = new Line(pts1[num], pts2[num]);
                        Curve bcrv = bLine.ToNurbsCurve();
                        tcrv1.DivideByCount(count, true, out bpts);

                        for (int m = 0; m < bpts.Count() - 1; m++)
                        {
                            Line hline = new Line(bpts[m], bpts[m + 1]);
                            SH_Node[] hnodes = new SH_Node[2];
                            hnodes[0] = new SH_Node(hline.From, null);
                            hnodes[1] = new SH_Node(hline.To, null);
                            SH_Line sh_hbSecondary = new SH_Line(hnodes, _ss.elementCount++, "hbFlatTruss_Mitchell_0_1"); //horizontal bottom lines for Secondary Roof Structure, substructure 0 Primary roof structure 1
                            _ss.Elements["Line"].Add(sh_hbSecondary);
                        }

                        // Top beams
                        Point3d movePts1 = new Point3d(pts1[num].X, pts1[num].Y, pts1[num].Y + h);
                        Point3d movePts2 = new Point3d(pts2[num].X, pts2[num].Y, pts1[num].Y + h);
                        Line tline = new Line(movePts1, movePts2);
                        Curve tcrv = tline.ToNurbsCurve();

                        //Count secondary roof structuree
                        Point3d[]bpPts; //top points
                        tcrv.DivideByCount(count, true, out bpPts);

                        //Vertical lines
                        for (int n = 0; n < bpts.Count(); n++)
                        {
                            Line vline = new Line(bpts[n], bpPts[n]);
                            SH_Node[] vnodes = new SH_Node[2];
                            vnodes[0] = new SH_Node(vline.From, null);
                            vnodes[1] = new SH_Node(vline.To, null);
                            SH_Line sh_vTrussSecondary = new SH_Line(vnodes, _ss.elementCount++, "vFlatTruss_Mitchell_0_1"); //verticals for trusses for Primary Roof Strucutre, substructure 0 Primary roof structure 1
                            _ss.Elements["Line"].Add(sh_vTrussSecondary);
                        }

                        //Top lines/beams
                        for (int p = 0; p < bpPts.Count() - 1; p++)
                        {
                            Line pLine = new Line(bpPts[p], bpPts[p + 1]);
                            SH_Node[] pnodes = new SH_Node[2];
                            pnodes[0] = new SH_Node(pLine.From, null);
                            pnodes[1] = new SH_Node(pLine.To, null);
                            SH_Line sh_pTrussPrimary = new SH_Line(pnodes, _ss.elementCount++, "pFlatTruss_Mitchell_0_1"); //top beams for trusses for Primary Roof Strucutre, substructure 0 Primary roof structure 1
                            _ss.Elements["Line"].Add(sh_pTrussPrimary);
                        }

                        //Trusses/diagonals
                        for (int d1 = 1; d1 < bpts.Count() / 2; d1++)
                        {
                            Line line = new Line(bpPts[d1], bpts[d1 + 1]);
                            SH_Node[] nodes = new SH_Node[2];
                            nodes[0] = new SH_Node(line.From, null);
                            nodes[1] = new SH_Node(line.To, null);
                            SH_Line sh_dTrussPrimary = new SH_Line(nodes, _ss.elementCount++, "dFlatTruss_Mitchell_0_1"); //diagonals for trusses for Primary Roof Strucutre, substructure 0 Primary roof structure 1
                            _ss.Elements["Line"].Add(sh_dTrussPrimary);
                        }
                        for (int d2 = bpts.Count() / 2; d2 < bpts.Count() - 2; d2++)
                        {
                            Line line = new Line(bpts[d2], bpPts[d2 + 1]);
                            SH_Node[] nodes = new SH_Node[2];
                            nodes[0] = new SH_Node(line.From, null);
                            nodes[1] = new SH_Node(line.To, null);
                            SH_Line sh_dTrussPrimary = new SH_Line(nodes, _ss.elementCount++, "dFlatTruss_Mitchell_0_1"); //diagonals for trusses for Primary Roof Strucutre, substructure 0 Primary roof structure 1
                            _ss.Elements["Line"].Add(sh_dTrussPrimary);
                        }

                        //store nodes from bpts and bpPts
                        for (int node1 = 0; node1 < bpts.Length; node1++)
                        {
                            SH_Node[] tnodes = new SH_Node[1];
                            tnodes[0] = new SH_Node(bpts[node1], null);
                            nodeLst.Add(tnodes[0]);
                        }
                        for (int node1 = 0; node1 < bpPts.Length; node1++)
                        {
                            SH_Node[] tnodes = new SH_Node[1];
                            tnodes[0] = new SH_Node(bpPts[node1], null);
                            nodeLst.Add(tnodes[0]);
                        }

                    }
                }
            }

              
                      // ------------- SECONDARY ROOF STRUCTURE FOR SUBSTRUCTURE 1 AND PRIMARY ROOF STRUCTURE 0 (TRUSS) ------------- 
                      if (nummberStructure[1] == "1" && nummberStructure[2] == "0")
                      {
                          //Collect the two elements that are the transversal beams (bottom beam and top beam)
                          //Bottom beams
                          var hbTBeams = from hbTBeam in _ss.Elements["Line"]
                                         where hbTBeam.elementName == "hbTrussPrimarySub1"
                                         select hbTBeam;

                          //Top beam¨s
                          var htTBeams = from htTBeam in _ss.Elements["Line"]
                                         where htTBeam.elementName == "htTrussPrimarySub1"
                                         select htTBeam;

                          //Store necessary points to construct pitched trusses
                          //Horizontal bottom points for one side
                          List<Point3d> hbPts1 = new List<Point3d>(); //list for bottom points
                          for (int el1 = 0; el1 < hbTBeams.Count() / 2; el1++)
                          {
                              //Nodes for bottom beams
                              SH_Element hb = hbTBeams.ElementAt(el1);
                              hbPts1.Add(hb.Nodes[0].Position);
                          }
                          SH_Element hb1 = hbTBeams.ElementAt(hbTBeams.Count() / 2 - 1);
                          hbPts1.Add(hb1.Nodes[1].Position);

                          //Horizontal bttom points for another side
                          List<Point3d> hbPts2 = new List<Point3d>(); //list for bottom points
                          for (int el2 = hbTBeams.Count() / 2; el2 < hbTBeams.Count(); el2++)
                          {
                              //Nodes for bottom beams
                              SH_Element hb = hbTBeams.ElementAt(el2);
                              hbPts2.Add(hb.Nodes[0].Position);
                          }
                          SH_Element hb2 = hbTBeams.Last();
                          hbPts2.Add(hb2.Nodes[1].Position);

                          //Horizontal top points for one side
                          List<Point3d> htPts1 = new List<Point3d>(); //list for top points
                          for (int el = 0; el < htTBeams.Count() / 2; el++)
                          {
                              //Nodes for top beams
                              SH_Element tb = htTBeams.ElementAt(el);
                              htPts1.Add(tb.Nodes[0].Position);
                          }
                          SH_Element tb1 = htTBeams.ElementAt(htTBeams.Count() / 2 - 1);
                          htPts1.Add(tb1.Nodes[1].Position);

                          //Horizontal top points for anpther side
                          List<Point3d> htPts2 = new List<Point3d>(); //list for top points
                          for (int el = htTBeams.Count() / 2; el < htTBeams.Count(); el++)
                          {
                              //Nodes for top beams
                              SH_Element tb = htTBeams.ElementAt(el);
                              htPts2.Add(tb.Nodes[0].Position);
                          }
                          SH_Element btb2 = htTBeams.Last();
                          htPts2.Add(btb2.Nodes[1].Position);

                          //---------------- PITCHED TRUSSES as secondary roof structure ----------------
                          if (nrSecondaryRS == 0)
                          {
                              for (int num = 0; num < hbPts1.Count(); num++)
                              {
                                  double hbeam = hbPts1[0].DistanceTo(htPts1[0]); //Height primary structure
                                  Line bl = new Line(hbPts1[num], hbPts2[num]); //bl = bottom line (bottom beam)
                                  Point3d midPt = bl.PointAt(0.5);
                                  Point3d tmidPt = new Point3d(midPt.X, midPt.Y, midPt.Z + hbeam + h); //highest point of secondary roof structure

                                  //Bottom beam
                                  Point3d[] bPts;
                                  Curve bcrv = bl.ToNurbsCurve();
                                  bcrv.DivideByCount(Convert.ToInt32(count), true, out bPts);

                                  //Inclined beams, pitched beams
                                  Point3d[] pPts;
                                  List<Point3d> ppts = new List<Point3d>();
                                  ppts.Add(htPts1[num]);
                                  ppts.Add(tmidPt);
                                  ppts.Add(htPts2[num]);
                                  Curve pcrv = Curve.CreateControlPointCurve(ppts, 1);
                                  pcrv.DivideByCount(Convert.ToInt32(count), true, out pPts);

                                  //Horizontal bottom lines
                                  for (int i = 0; i < bPts.Count() - 1; i++)
                                  {
                                      Line hbline = new Line(bPts[i], bPts[i + 1]);
                                      SH_Node[] hbnodes = new SH_Node[2];
                                      hbnodes[0] = new SH_Node(hbline.From, null);
                                      hbnodes[1] = new SH_Node(hbline.To, null);
                                      SH_Line sh_hbTrussSecondary = new SH_Line(hbnodes, _ss.elementCount++, "hbPitchedTruss_Mitchell_1_0"); //bottom horizontals for trusses for Secondary Roof Strucutre, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_hbTrussSecondary);
                                  }

                                  //Vertical lines
                                  for (int n = 1; n < bPts.Count() - 1; n++)
                                  {
                                      Line vline = new Line(bPts[n], pPts[n]);
                                      SH_Node[] vnodes = new SH_Node[2];
                                      vnodes[0] = new SH_Node(vline.From, null);
                                      vnodes[1] = new SH_Node(vline.To, null);
                                      SH_Line sh_vTrussSecondary = new SH_Line(vnodes, _ss.elementCount++, "vPitchedTruss_Mitchell_1_0"); //verticals for trusses for Secondary Roof Strucutre, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_vTrussSecondary);
                                  }

                                  //Pitched lines
                                  for (int j = 0; j < pPts.Count() - 1; j++)
                                  {
                                      Line pLine = new Line(pPts[j], pPts[j + 1]);
                                      SH_Node[] pnodes = new SH_Node[2];
                                      pnodes[0] = new SH_Node(pLine.From, null);
                                      pnodes[1] = new SH_Node(pLine.To, null);
                                      SH_Line sh_pTrussSecondary = new SH_Line(pnodes, _ss.elementCount++, "pPitchedTruss_Mitchell_1_0"); //inclined lined  for trusses for Secondary Roof Strucutre, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_pTrussSecondary);
                                  }

                                  //Trusses/diagonals
                                  for (int k = 1; k < bPts.Count() / 2; k++)
                                  {
                                      Line line = new Line(pPts[k], bPts[k + 1]);
                                      SH_Node[] nodes = new SH_Node[2];
                                      nodes[0] = new SH_Node(line.From, null);
                                      nodes[1] = new SH_Node(line.To, null);
                                      SH_Line sh_dTrussSecondary = new SH_Line(nodes, _ss.elementCount++, "dPitchedTruss_Mitchell_1_0"); //diagonals for trusses for Secondary Roof Strucutre, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_dTrussSecondary);
                                  }
                                  for (int m = bPts.Count() / 2; m < bPts.Count() - 2; m++)
                                  {
                                      Line line = new Line(bPts[m], pPts[m + 1]);
                                      SH_Node[] nodes = new SH_Node[2];
                                      nodes[0] = new SH_Node(line.From, null);
                                      nodes[1] = new SH_Node(line.To, null);
                                      SH_Line sh_dTrussSecondary = new SH_Line(nodes, _ss.elementCount++, "dPitchedTruss_Mitchell_1_0"); //diagonals for trusses for Secondary Roof Strucutre, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_dTrussSecondary);
                                  }

                                  //store nodes from bPts and pPts
                                  for (int node1 = 0; node1 < bPts.Length; node1++)
                                  {
                                      SH_Node[] bnodes = new SH_Node[1];
                                      bnodes[0] = new SH_Node(bPts[node1], null);
                                      nodeLst.Add(bnodes[0]);
                                  }
                                  for (int node1 = 0; node1 < pPts.Length; node1++)
                                  {
                                      SH_Node[] pnodes = new SH_Node[1];
                                      pnodes[0] = new SH_Node(pPts[node1], null);
                                      nodeLst.Add(pnodes[0]);
                                  }
                              }
                          }
                          //---------------- BOWED TRUSSES as secondary roof structure ----------------
                          if (nrSecondaryRS == 1)
                          {
                              for (int num = 0; num < hbPts1.Count(); num++)
                              {
                                  double hbeam = hbPts1[0].DistanceTo(htPts1[0]); //Height primary structure
                                  Line bl = new Line(hbPts1[num], hbPts2[num]); //bl = bottom line (bottom beam)
                                  Point3d midPt = bl.PointAt(0.5);
                                  Point3d tmidPt = new Point3d(midPt.X, midPt.Y, midPt.Z + hbeam + h); //highest point of secondary roof structure

                                  //Bottom beam
                                  Point3d[] bPts;
                                  Curve bcrv = bl.ToNurbsCurve();
                                  bcrv.DivideByCount(Convert.ToInt32(count), true, out bPts);

                                  //Arch
                                  Point3d[] bowedPts;
                                  List<Point3d> ppts = new List<Point3d>();
                                  ppts.Add(htPts1[num]);
                                  ppts.Add(tmidPt);
                                  ppts.Add(htPts2[num]);
                                  Curve pcrv = Curve.CreateControlPointCurve(ppts, 2);
                                  pcrv.DivideByCount(Convert.ToInt32(count), true, out bowedPts);

                                  //Horizontal bottom lines
                                  for (int i = 0; i < bPts.Count() - 1; i++)
                                  {
                                      Line hbline = new Line(bPts[i], bPts[i + 1]);
                                      SH_Node[] hbnodes = new SH_Node[2];
                                      hbnodes[0] = new SH_Node(hbline.From, null);
                                      hbnodes[1] = new SH_Node(hbline.To, null);
                                      SH_Line sh_hbTrussSecondary = new SH_Line(hbnodes, _ss.elementCount++, "hbBowedTruss_Mitchell_1_0"); //Bottom horizontals for trusses for Secondary Roof Structure, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_hbTrussSecondary);
                                  }

                                  //Vertical lines
                                  for (int n = 1; n < bPts.Count() - 1; n++)
                                  {
                                      Line vline = new Line(bPts[n], bowedPts[n]);
                                      SH_Node[] vnodes = new SH_Node[2];
                                      vnodes[0] = new SH_Node(vline.From, null);
                                      vnodes[1] = new SH_Node(vline.To, null);
                                      SH_Line sh_vTrussSecondary = new SH_Line(vnodes, _ss.elementCount++, "vBowedTruss_Mitchell_1_0"); //Verticals for trusses for Secondary Roof Structure, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_vTrussSecondary);
                                  }

                                  //Bowed lines
                                  for (int j = 0; j < bowedPts.Count() - 1; j++)
                                  {
                                      Line pLine = new Line(bowedPts[j], bowedPts[j + 1]);
                                      SH_Node[] pnodes = new SH_Node[2];
                                      pnodes[0] = new SH_Node(pLine.From, null);
                                      pnodes[1] = new SH_Node(pLine.To, null);
                                      SH_Line sh_pTrussSecondary = new SH_Line(pnodes, _ss.elementCount++, "pBowedTruss_Mitchell_1_0"); //Bowed lines  for trusses for Secondary Roof Structure, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_pTrussSecondary);
                                  }

                                  //Trusses/diagonals
                                  for (int k = 1; k < bPts.Count() / 2; k++)
                                  {
                                      Line line = new Line(bowedPts[k], bPts[k + 1]);
                                      SH_Node[] nodes = new SH_Node[2];
                                      nodes[0] = new SH_Node(line.From, null);
                                      nodes[1] = new SH_Node(line.To, null);
                                      SH_Line sh_dTrussSecondary = new SH_Line(nodes, _ss.elementCount++, "dBowedTruss_Mitchell_1_0"); //Diagonals for trusses for Secondary Roof Structure, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_dTrussSecondary);
                                  }
                                  for (int m = bPts.Count() / 2; m < bPts.Count() - 2; m++)
                                  {
                                      Line line = new Line(bPts[m], bowedPts[m + 1]);
                                      SH_Node[] nodes = new SH_Node[2];
                                      nodes[0] = new SH_Node(line.From, null);
                                      nodes[1] = new SH_Node(line.To, null);
                                      SH_Line sh_dTrussSecondary = new SH_Line(nodes, _ss.elementCount++, "dBowwedTruss_Mitchell_1_0"); //Diagonals for trusses for Secondary Roof Structure, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_dTrussSecondary);
                                  }

                                  //store nodes from bPts and pPts
                                  for (int node1 = 0; node1 < bPts.Length; node1++)
                                  {
                                      SH_Node[] bnodes = new SH_Node[1];
                                      bnodes[0] = new SH_Node(bPts[node1], null);
                                      nodeLst.Add(bnodes[0]);
                                  }
                                  for (int node1 = 0; node1 < bowedPts.Length; node1++)
                                  {
                                      SH_Node[] pnodes = new SH_Node[1];
                                      pnodes[0] = new SH_Node(bowedPts[node1], null);
                                      nodeLst.Add(pnodes[0]);
                                  }
                              }
                          }

                          //---------------- BEAMS as secondary roof structure ----------------
                          if (nrSecondaryRS == 2)
                          {
                              for (int num = 0; num < htPts1.Count(); num++)
                              {
                                  Line secBeam = new Line(htPts1[num], htPts2[num]);
                                  SH_Node[] nodes = new SH_Node[2];
                                  nodes[0] = new SH_Node(secBeam.From, null);
                                  nodes[1] = new SH_Node(secBeam.To, null);
                                  SH_Line sh_BeamsSecondary = new SH_Line(nodes, _ss.elementCount++, "Beams_Mitchell_1_0"); //Horiszontal Beams for Secondary Roof Structure, substructure 1 Primary roof structure 0
                                  _ss.Elements["Line"].Add(sh_BeamsSecondary);
                              }
                          }

                          //---------------- FLAT TRUSSES as secondary roof structure ----------------
                          if (nrSecondaryRS == 3)
                          {
                              for (int num = 0; num < hbPts1.Count(); num++)
                              {
                                  double hbeam = hbPts1[0].DistanceTo(htPts1[0]); //Height primary structure
                                  Line bl = new Line(hbPts1[num], hbPts2[num]); //bl = bottom line (bottom beam)

                                  //Bottom beam
                                  Point3d[] bPts;
                                  Curve bcrv = bl.ToNurbsCurve();
                                  bcrv.DivideByCount(Convert.ToInt32(count), true, out bPts);

                                  //Top
                                  Point3d[] tPts;
                                  List<Point3d> ppts = new List<Point3d>();
                                  Line l = new Line(htPts1[num], htPts2[num]);
                                  Curve tcrv = l.ToNurbsCurve();
                                  tcrv.DivideByCount(Convert.ToInt32(count), true, out tPts);

                                  //Horizontal bottom lines
                                  for (int i = 0; i < bPts.Count() - 1; i++)
                                  {
                                      Line hbline = new Line(bPts[i], bPts[i + 1]);
                                      SH_Node[] hbnodes = new SH_Node[2];
                                      hbnodes[0] = new SH_Node(hbline.From, null);
                                      hbnodes[1] = new SH_Node(hbline.To, null);
                                      SH_Line sh_hbTrussSecondary = new SH_Line(hbnodes, _ss.elementCount++, "hbFlatTruss_Mitchell_1_0"); //bottom horizontals for trusses for Secondary Roof Structure, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_hbTrussSecondary);
                                  }

                                  //Vertical lines
                                  for (int n = 1; n < bPts.Count() - 1; n++)
                                  {
                                      Line vline = new Line(bPts[n], tPts[n]);
                                      SH_Node[] vnodes = new SH_Node[2];
                                      vnodes[0] = new SH_Node(vline.From, null);
                                      vnodes[1] = new SH_Node(vline.To, null);
                                      SH_Line sh_vTrussSecondary = new SH_Line(vnodes, _ss.elementCount++, "vFlatTruss_Mitchell_1_0"); //verticals for trusses for Secondary Roof Structure, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_vTrussSecondary);
                                  }

                                  //Horizontal top lines
                                  for (int j = 0; j < tPts.Count() - 1; j++)
                                  {
                                      Line tLine = new Line(tPts[j], tPts[j + 1]);
                                      SH_Node[] nodes = new SH_Node[2];
                                      nodes[0] = new SH_Node(tLine.From, null);
                                      nodes[1] = new SH_Node(tLine.To, null);
                                      SH_Line sh_pTrussSecondary = new SH_Line(nodes, _ss.elementCount++, "pFlatTruss_Mitchell_1_0"); //top horizontals  for trusses for Secondary Roof Structure, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_pTrussSecondary);
                                  }

                                  //Trusses/diagonals
                                  for (int k = 1; k < bPts.Count() - 1; k += 2)
                                  {
                                      Line line = new Line(tPts[k], bPts[k + 1]);
                                      SH_Node[] nodes = new SH_Node[2];
                                      nodes[0] = new SH_Node(line.From, null);
                                      nodes[1] = new SH_Node(line.To, null);
                                      SH_Line sh_dTrussSecondary = new SH_Line(nodes, _ss.elementCount++, "dFlatTruss_Mitchell_1_0"); //diagonals for trusses for Secondary Roof Structure, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_dTrussSecondary);
                                  }
                                  for (int m = 0; m < bPts.Count() - 2; m += 2)
                                  {
                                      Line line = new Line(bPts[m], tPts[m + 1]);
                                      SH_Node[] nodes = new SH_Node[2];
                                      nodes[0] = new SH_Node(line.From, null);
                                      nodes[1] = new SH_Node(line.To, null);
                                      SH_Line sh_dTrussSecondary = new SH_Line(nodes, _ss.elementCount++, "dFlatTruss_Mitchell_1_0"); //diagonals for trusses for Secondary Roof Structure, substructure 1 Primary roof structure 0
                                      _ss.Elements["Line"].Add(sh_dTrussSecondary);
                                  }

                                  //store nodes from bPts and pPts
                                  for (int node1 = 0; node1 < bPts.Length; node1++)
                                  {
                                      SH_Node[] bnodes = new SH_Node[1];
                                      bnodes[0] = new SH_Node(bPts[node1], null);
                                      nodeLst.Add(bnodes[0]);
                                  }
                                  for (int node1 = 0; node1 < tPts.Length; node1++)
                                  {
                                      SH_Node[] pnodes = new SH_Node[1];
                                      pnodes[0] = new SH_Node(tPts[node1], null);
                                      nodeLst.Add(pnodes[0]);
                                  }
                              }
                          }
                      }
           
            
                      // ------------- SECONDARY ROOF STRUCTURE FOR SUBSTRUCTURE 0 AND PRIMARY ROOF STRUCTURE 1 (SOLID BEAM) -------------
                      if (nummberStructure[1] == "1" && nummberStructure[2] == "1")
                      {
                       //Collect the two elements that are the transversal beams (bottom beam and top beam)
                       //Transversal beams
                       var tBeams = from tBeam in _ss.Elements["Line"]
                             where tBeam.elementName == "transBeamSub1"
                             select tBeam;

                       //End points at first beam
                        Point3d pt11 = tBeams.ElementAt(0).Nodes[0].Position;
                        Point3d pt12 = tBeams.ElementAt(0).Nodes[1].Position;

                         //End points at second beam
                         Point3d pt21 = tBeams.ElementAt(1).Nodes[0].Position;
                         Point3d pt22 = tBeams.ElementAt(1).Nodes[1].Position;

                         //Construct transversal beams, divide them repsect to count
                         Point3d[] pts1;
                         Line tLine1 = new Line(pt11, pt12);
                         Curve tcrv1 = tLine1.ToNurbsCurve();
                         tcrv1.DivideByCount(numberSecondaryRS, true, out pts1);

                         Point3d[] pts2;
                         Line tLine2 = new Line(pt21, pt22);
                         Curve tcrv2 = tLine2.ToNurbsCurve();
                         tcrv2.DivideByCount(numberSecondaryRS, true, out pts2);

                        //store nodes from pts1 and pts2, end nodes already stored
                         for (int node1 = 1; node1 < pts1.Length - 1; node1++)
                         {
                         SH_Node[] nodes1 = new SH_Node[1];
                         nodes1[0] = new SH_Node(pts1[node1], null);
                         nodeLst.Add(nodes1[0]);
                         }
                        for (int node1 = 1; node1 < pts2.Length - 1; node1++)
                         {
                          SH_Node[] nodes2 = new SH_Node[1];
                          nodes2[0] = new SH_Node(pts2[node1], null);
                          nodeLst.Add(nodes2[0]);
                        }

                        //Horizontal transversal lines
                        for (int i = 0; i < pts1.Count() - 1; i++)
                         {
                         Line htline = new Line(pts1[i], pts1[i + 1]);
                         SH_Node[] htnodes = new SH_Node[2];
                         htnodes[0] = new SH_Node(htline.From, null);
                         htnodes[1] = new SH_Node(htline.To, null);
                         SH_Line sh_htSecondary = new SH_Line(htnodes, _ss.elementCount++, "hTransPitchedTruss_Mitchell_1_1"); //horizontal trransversals for Secondary Roof Structure, substructure 0 Primary roof structure 1
                         _ss.Elements["Line"].Add(sh_htSecondary);
                         }

                       for (int j = 0; j < pts2.Count() - 1; j++)
                       {
                        Line htline = new Line(pts2[j], pts2[j + 1]);
                        SH_Node[] htnodes = new SH_Node[2];
                        htnodes[0] = new SH_Node(htline.From, null);
                        htnodes[1] = new SH_Node(htline.To, null);
                        SH_Line sh_htSecondary = new SH_Line(htnodes, _ss.elementCount++, "hTransPitchedTruss_Mitchell_1_1"); //horizontal transversals for Secondary Roof Structure, substructure 0 Primary roof structure 1
                        _ss.Elements["Line"].Add(sh_htSecondary);
                        }


                      //---------------- PITCHED TRUSSES as secondary roof structure ----------------

                      //---------------- BOWED TRUSSES as secondary roof structure ----------------

                       //---------------- BEAMS as secondary roof structure ----------------
                       //OK TESTET
                      if (nrSecondaryRS == 2)
                       {
                        //Construct the secondary beams
                        for (int k = 0; k < pts1.Length; k++)
                        {
                        Line bLine = new Line(pts1[k], pts2[k]);           
                        SH_Node[] hnodes = new SH_Node[2];
                        hnodes[0] = new SH_Node(bLine.From, null);
                        hnodes[1] = new SH_Node(bLine.To, null);
                        SH_Line sh_hbSecondary = new SH_Line(hnodes, _ss.elementCount++, "hBeams_Mitchell_1_1"); //horizontal bottom lines for Secondary Roof Structure, substructure 0 Primary roof structure 1
                        _ss.Elements["Line"].Add(sh_hbSecondary);                         
                         }
                       }

                          //---------------- FLAT TRUSSES as secondary roof structure ----------------
                      }
                      
            
                      // ------------- SECONDARY ROOF STRUCTURE FOR SUBSTRUCTURE 2 AND PRIMARY ROOF STRUCTURE 2 (PITCHED ROOF ) -------------
                      if (nummberStructure[1] == "2")
                      {
                          // ----------------JOIST as secondary roof structure ----------------
                          var pitchedBeams = from pBeam in _ss.Elements["Line"]
                                             where pBeam.elementName == "pTrussPrimarySub2"
                                             select pBeam;

                          List<Point3d> pts1 = new List<Point3d>();
                          pts1.Add(pitchedBeams.ElementAt(0).Nodes[0].Position);
                          pts1.Add(pitchedBeams.ElementAt(pitchedBeams.Count() / 4).Nodes[0].Position);
                          pts1.Add(pitchedBeams.ElementAt(pitchedBeams.Count()/2-1).Nodes[1].Position);

                          Curve crv1 = Curve.CreateControlPointCurve(pts1, 1);
                          Point3d[] pPts1;
                          crv1.DivideByCount(numberSecondaryRS+1, true, out pPts1);

                          List<Point3d> pts2 = new List<Point3d>();
                          pts2.Add(pitchedBeams.ElementAt(pitchedBeams.Count() / 2).Nodes[0].Position);
                          pts2.Add(pitchedBeams.ElementAt((pitchedBeams.Count() / 2) + (pitchedBeams.Count() / 4)).Nodes[0].Position);
                          pts2.Add(pitchedBeams.Last().Nodes[1].Position);

                          Curve crv2 = Curve.CreateControlPointCurve(pts2, 1);
                          Point3d[] pPts2;
                          crv2.DivideByCount(numberSecondaryRS+1, true, out pPts2);

                          for (int i = 0; i< pPts1.Count();i++)
                          {
                              Line jLine = new Line(pPts1[i], pPts2[i]);
                              SH_Node[] nodes = new SH_Node[2];
                              nodes[0] = new SH_Node(jLine.From, null);
                              nodes[1] = new SH_Node(jLine.To, null);
                              SH_Line sh_joistSecondary = new SH_Line(nodes, _ss.elementCount++, "joist_Mitchell_1_2"); //Joist for Secondary Roof Structure, substructure 1 Primary roof structure 2
                              _ss.Elements["Line"].Add(sh_joistSecondary);
                          }

                      }

            // ------------- SECONDARY ROOF STRUCTUREE FOR SUBSTRUCTURE 3 (bowed roof) ------------- 
            if (nummberStructure[1] == "3")
            {
                // ----------------JOIST as secondary roof structure ----------------
                var bowedBeams = from bBeam in _ss.Elements["Line"]
                                 where bBeam.elementName == "bowedRoofSub3"
                                 select bBeam;

                List<Point3d> bowedPts = new List<Point3d>();
                
                foreach (SH_Element el in bowedBeams)
                {
                    Line bLine = new Line(el.Nodes[0].Position, el.Nodes[1].Position);
                    Curve bcrv = bLine.ToNurbsCurve();
                    Point3d[] pts;
                    bcrv.DivideByCount(Convert.ToInt32(numberSecondaryRS), true, out pts);
                    bowedPts.AddRange(pts);
                }

                for (int pt = 0; pt < bowedPts.Count() / 2 - 1; pt++)
                {
                    Line jLine = new Line(bowedPts[pt], bowedPts[pt + bowedPts.Count()/2]);
                    SH_Node[] nodes = new SH_Node[2];
                    nodes[0] = new SH_Node(jLine.From, null);
                    nodes[1] = new SH_Node(jLine.To, null);
                    SH_Line sh_joistSecondary = new SH_Line(nodes, _ss.elementCount++, "joist_Mitchell_1_3"); //Joist for Secondary Roof Structure, substructure 1 Primary roof structure 3
                    _ss.Elements["Line"].Add(sh_joistSecondary);
                }
            }


            //Add all nodes to SH_Elements
            _ss.Nodes = new List<SH_Node>();
            _ss.Nodes.AddRange(nodeLst);

            // change the state
            _ss.SimpleShapeState = State.epsilon;
            return "PrimaryRoofStructure successfully applied.";

        }

        public override State GetNextState()
        {
            return State.epsilon;
        }

    }
}