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
        public double nrPrimaryRS;
        public double hBeam;
        public double count;

        // --- constructors ---
        public PrimaryRoofStructureRule()
        {
            Name = "PrimaryRoofStructureClass";
            RuleState = State.gamma;
        }

        public PrimaryRoofStructureRule(double _nrPrimaryRS, double _hBeam, double _count)
        {
            nrPrimaryRS = _nrPrimaryRS;
            hBeam = _hBeam;
            count = _count;
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
            // check if the state maches the simple shape state
            if (_ss.SimpleShapeState != RuleState)
            {
                return "The State is not compatible with PrimaryRoofStructureRule.";
            }

            // Get the substructure number from MRule2
            string name = _ss.name;
            string[] nrSub = name.Split('_');

            List<SH_Node> nodeLst = new List<SH_Node>(); //list for nodes

            // ------------- Primary roof structure for substructure 0 ------------- 
            if (nrSub[1] == "0")
            {
                // ------------- Truss ------------
                if (nrPrimaryRS == 0) 
                {
                    _ss.name += "_0"; //Add number of primary substructure to _ss.name

                    //Search for the two elements that are the transversal beams
                    var tBeams = from tBeam in _ss.Elements["Line"]
                                 where tBeam.elementName == "transBeamSub0"
                                 select tBeam;

                    foreach (SH_Element tb in tBeams.ToList())
                    {
                        Point3d p1 = tb.Nodes[0].Position; //node 1 coordinates
                        Point3d p2 = tb.Nodes[1].Position; //node 1 coordinates
                        Point3d np1 = new Point3d(p1.X, p1.Y, p1.Z + hBeam); //new point 1 (move in z dir)
                        Point3d np2 = new Point3d(p2.X, p2.Y, p2.Z + hBeam); //new point 2 (move in z dir)

                        // new SH_node
                        SH_Node[] nodes = new SH_Node[2];
                        nodes[0] = new SH_Node(np1, null);
                        nodes[1] = new SH_Node(np2, null);

                        //add nodes to node list
                        nodeLst.Add(nodes[0]);
                        nodeLst.Add(nodes[1]);

                        //new beam, constructed bases on the first transversal beam
                        //SH_Line sh_line = new SH_Line(nodes, _ss.elementCount++, "nTransBeamSub0");
                        //_ss.Elements["Line"].Add(sh_line);
                        Line tline = new Line(np1 , np2); //top beam/line

                        //Divide beams and sort list, thereby construct lines
                        Point3d[] trussPts1;
                        Curve tLine = tline.ToNurbsCurve(); //top beam/line
                        tLine.DivideByCount(Convert.ToInt32(count), true, out trussPts1);

                        //sort direction of list
                        List<Point3d> sortedtrussPts1 = new List<Point3d>();
                        if (tLine.PointAtEnd.X == tLine.PointAtStart.X)
                        {
                            sortedtrussPts1 = trussPts1.OrderBy(pt => pt.Y).ToList();
                        }
                        else if (tLine.PointAtEnd.Y == tLine.PointAtStart.Y)
                        {
                            sortedtrussPts1 = trussPts1.OrderBy(pt => pt.X).ToList();
                        }

                        Point3d[] trussPts2;
                        Line bline = new Line(p1, p2);
                        Curve bLine = bline.ToNurbsCurve(); //bottom beam/line (original)
                        bLine.DivideByCount(Convert.ToInt32(count), true, out trussPts2);

                        //sort direction of list
                        List<Point3d> sortedtrussPts2 = new List<Point3d>();
                        if (bLine.PointAtEnd.X == bLine.PointAtStart.X)
                        {
                            sortedtrussPts2 = trussPts2.OrderBy(pt => pt.Y).ToList();
                        }
                        else if (bLine.PointAtEnd.Y == bLine.PointAtStart.Y)
                        {
                            sortedtrussPts2 = trussPts2.OrderBy(pt => pt.X).ToList();
                        }

                        //horizontal top lines
                        for (int i = 0; i < sortedtrussPts1.Count()-1; i++)
                        {
                            Line htLine = new Line(sortedtrussPts1[i], sortedtrussPts1[i+1]);
                            SH_Node[] htnodes = new SH_Node[2];
                            htnodes[0] = new SH_Node(htLine.From, null);
                            htnodes[1] = new SH_Node(htLine.To, null);
                            SH_Line sh_htTrussPrimary = new SH_Line(htnodes, _ss.elementCount++, "htTrussPrimarySub0"); //top horizontals for truss beam for Primary Roof Strucutre, substructure 0
                            _ss.Elements["Line"].Add(sh_htTrussPrimary);
                        }

                        //horizontal bottom lines
                        for (int j = 0; j < sortedtrussPts2.Count()-1; j++)
                        {
                            Line bhLine = new Line(sortedtrussPts2[j], sortedtrussPts2[j + 1]);
                            SH_Node[] hbnodes = new SH_Node[2];
                            hbnodes[0] = new SH_Node(bhLine.From, null);
                            hbnodes[1] = new SH_Node(bhLine.To, null);
                            SH_Line sh_hbTrussPrimary = new SH_Line(hbnodes, _ss.elementCount++, "hbTrussPrimarySub0"); //bottom horizontals for truss beam for Primary Roof Strucutre, substructure 0
                            _ss.Elements["Line"].Add(sh_hbTrussPrimary);
                        }

                        //vertical lines
                        for (int k = 0; k < sortedtrussPts1.Count(); k++)
                        {
                            Line vLine = new Line(sortedtrussPts1[k], sortedtrussPts2[k]);
                            SH_Node[] vnodes = new SH_Node[2];
                            vnodes[0] = new SH_Node(vLine.From, null);
                            vnodes[1] = new SH_Node(vLine.To, null);
                            SH_Line sh_vTrussPrimary = new SH_Line(vnodes, _ss.elementCount++, "vTrussPrimarySub0"); //verticals for truss beam for Primary Roof Strucutre, substructure 0
                            _ss.Elements["Line"].Add(sh_vTrussPrimary);
                        }

                        //diagonal lines
                        for (int i = 0; i < sortedtrussPts2.Count() - 1; i += 2)
                        {
                            Line dLine = new Line(sortedtrussPts2[i], sortedtrussPts1[i+1]);
                            SH_Node[] dnodes = new SH_Node[2];
                            dnodes[0] = new SH_Node(dLine.From, null);
                            dnodes[1] = new SH_Node(dLine.To, null);
                            SH_Line sh_dTrussPrimary = new SH_Line(dnodes, _ss.elementCount++, "dTrussPrimarySub0"); //diagonals for truss beam for Primary Roof Strucutre, substructure 0
                            _ss.Elements["Line"].Add(sh_dTrussPrimary);
                        }
                        for (int j = 1; j < sortedtrussPts1.Count() - 1; j += 2)
                        {
                            Line dLine = new Line(sortedtrussPts1[j], sortedtrussPts2[j + 1]);
                            SH_Node[] dnodes = new SH_Node[2];
                            dnodes[0] = new SH_Node(dLine.From, null);
                            dnodes[1] = new SH_Node(dLine.To, null);
                            SH_Line sh_dTrussPrimary = new SH_Line(dnodes, _ss.elementCount++, "dTrussPrimarySub0"); //diagonals for truss beam for Primary Roof Strucutre, substructure 0
                            _ss.Elements["Line"].Add(sh_dTrussPrimary);
                        }

                        //store nodes from sortedtrussPts1 and sortedtrussPts2
                        for (int node1 = 0; node1 < sortedtrussPts1.Count; node1++)
                        {
                            SH_Node[] tnodes = new SH_Node[1];
                            tnodes[0] = new SH_Node(sortedtrussPts1[node1], null);
                            nodeLst.Add(tnodes[0]);
                        }
                        for (int node1 = 0; node1 < sortedtrussPts2.Count; node1++)
                        {
                            SH_Node[] tnodes = new SH_Node[1];
                            tnodes[0] = new SH_Node(sortedtrussPts2[node1], null);
                            nodeLst.Add(tnodes[0]);
                        }
                    }
                    //Remove transversal beams
                    _ss.Elements["Line"].RemoveAll(el => el.elementName == "transBeamSub0");

                }

                if (nrPrimaryRS == 1) // Beam (solid)
                {
                    _ss.name += "_1";

                    /*
                    //Search for the two elements that are the transversal beams
                    var tBeams = from tBeam in _ss.Elements["Line"]
                                 where tBeam.elementName == "transBeamSub0"
                                 select tBeam;

                    foreach (SH_Element tb in tBeams.ToList())
                    {
                        List<Point3d> pts1 = new List<Point3d>(); //empty list to store points, before sorting them
                        List<Point3d> pts2 = new List<Point3d>(); //empty list to store points, before sorting them
                        Point3d p1 = tb.Nodes[0].Position; //node 1 coordinates
                        Point3d p2 = tb.Nodes[1].Position; //node 1 coordinates
                        pts1.Add(p1);
                        pts1.Add(p2);
                        Point3d np1 = new Point3d(p1.X, p1.Y, p1.Z + hBeam); //new point 1 (move in z dir)
                        Point3d np2 = new Point3d(p2.X, p2.Y, p2.Z + hBeam); //new point 2 (move in z dir)
                        pts2.Add(np1);
                        pts2.Add(np2);

                        // new SH_node
                        SH_Node[] nodes = new SH_Node[2];
                        nodes[0] = new SH_Node(np1, null);
                        nodes[1] = new SH_Node(np2, null);
                        
                        //nodes.Add(node[0]); //add nodes to the same node list

                        //new beam, constructed bases on the first transversal beam
                        SH_Line sh_line = new SH_Line(nodes, _ss.elementCount++, "nTransBeamSub0");
                        _ss.Elements["Line"].Add(sh_line);

                        //Sort list, thereby construct lines              
                        Curve tLine = sh_line.NurbsCurve; //top beam/line
                        if (tLine.PointAtEnd.X == tLine.PointAtStart.X)
                        {
                            pts1.OrderBy(pt => pt.Y);
                        }
                        else if (tLine.PointAtEnd.Y == tLine.PointAtStart.Y)
                        {
                            pts1.OrderBy(pt => pt.X);
                        }

                        //må være en lettere måte her
                        SH_Node[] node = tb.Nodes;
                        SH_Line sh_line2 = new SH_Line(node, _ss.elementCount++, "");
                        Curve bLine = sh_line2.NurbsCurve; //bottom beam/line (original)
                        if (bLine.PointAtEnd.X == bLine.PointAtStart.X)
                        {
                            pts2.OrderBy(pt => pt.Y);
                        }
                        else if (bLine.PointAtEnd.Y == bLine.PointAtStart.Y)
                        {
                            pts2.OrderBy(pt => pt.X);
                        }

                        //vertical lines
                        for (int k = 0; k < pts1.Count(); k++)
                        {
                            Line vLine = new Line(pts1[k], pts2[k]);
                            SH_Node[] vnodes = new SH_Node[2];
                            vnodes[0] = new SH_Node(vLine.From, null);
                            vnodes[1] = new SH_Node(vLine.To, null);
                            SH_Line sh_vTrussPrimary = new SH_Line(vnodes, _ss.elementCount++, "vBeamPrimarySub0"); //verticals for truss beam for Primary Roof Strucutre, substructure 0
                            _ss.Elements["Line"].Add(sh_vTrussPrimary);
                        }                     
                    }*/
                }
            }
            // ------------- Primary roof structure for substructure 1 ------------- 
            if (nrSub[1] == "1")
            {
                if (nrPrimaryRS == 0) // Truss
                {
                    _ss.name += "_0";

                    //Search for the two elements that are the transversal beams
                    var tBeams = from tBeam in _ss.Elements["Line"]
                                 where tBeam.elementName == "transBeamSub1"
                                 select tBeam;

                    foreach (SH_Element tb in tBeams.ToList())
                    {
                        Point3d p1 = tb.Nodes[0].Position; //node 1 coordinates
                        Point3d p2 = tb.Nodes[1].Position; //node 1 coordinates
                        Point3d np1 = new Point3d(p1.X, p1.Y, p1.Z + hBeam); //new point 1 (move in z dir)
                        Point3d np2 = new Point3d(p2.X, p2.Y, p2.Z + hBeam); //new point 2 (move in z dir)

                        // new SH_node
                        SH_Node[] nodes = new SH_Node[2];
                        nodes[0] = new SH_Node(np1, null);
                        nodes[1] = new SH_Node(np2, null);

                        nodeLst.Add(nodes[0]);
                        nodeLst.Add(nodes[1]);

                        //new top beam
                        Line tline = new Line(np1, np2);

                        //Divide beams and sort list, thereby construct lines
                        Point3d[] trussPts1;
                        Curve tLine = tline.ToNurbsCurve(); //top beam/line
                        tLine.DivideByCount(Convert.ToInt32(count), true, out trussPts1);
                        List<Point3d> sortedtrussPts1 = new List<Point3d>();
                        if (tLine.PointAtEnd.X == tLine.PointAtStart.X)
                        {
                            sortedtrussPts1 = trussPts1.OrderBy(pt => pt.Y).ToList();
                        }
                        else if (tLine.PointAtEnd.Y == tLine.PointAtStart.Y)
                        {
                            sortedtrussPts1 = trussPts1.OrderBy(pt => pt.X).ToList();
                        }

                        Point3d[] trussPts2;
                        Line bline = new Line(p1, p2);
                        Curve bLine = bline.ToNurbsCurve(); //bottom beam/line (original)
                        bLine.DivideByCount(Convert.ToInt32(count), true, out trussPts2);
                        List<Point3d> sortedtrussPts2 = new List<Point3d>();
                        if (bLine.PointAtEnd.X == bLine.PointAtStart.X)
                        {
                            sortedtrussPts2 = trussPts2.OrderBy(pt => pt.Y).ToList();
                        }
                        else if (bLine.PointAtEnd.Y == bLine.PointAtStart.Y)
                        {
                            sortedtrussPts2 = trussPts2.OrderBy(pt => pt.X).ToList();
                        }

                        //horizontal top lines
                        for (int i = 0; i < sortedtrussPts1.Count() - 1; i++)
                        {
                            Line htLine = new Line(sortedtrussPts1[i], sortedtrussPts1[i + 1]);
                            SH_Node[] htnodes = new SH_Node[2];
                            htnodes[0] = new SH_Node(htLine.From, null);
                            htnodes[1] = new SH_Node(htLine.To, null);
                            SH_Line sh_htTrussPrimary = new SH_Line(htnodes, _ss.elementCount++, "htTrussPrimarySub1"); //top horizontals for truss beam for Primary Roof Strucutre, substructure 0
                            _ss.Elements["Line"].Add(sh_htTrussPrimary);
                        }

                        //horizontal bottom lines
                        for (int j = 0; j < sortedtrussPts2.Count() - 1; j++)
                        {
                            Line bhLine = new Line(sortedtrussPts2[j], sortedtrussPts2[j + 1]);
                            SH_Node[] hbnodes = new SH_Node[2];
                            hbnodes[0] = new SH_Node(bhLine.From, null);
                            hbnodes[1] = new SH_Node(bhLine.To, null);
                            SH_Line sh_hbTrussPrimary = new SH_Line(hbnodes, _ss.elementCount++, "hbTrussPrimarySub1"); //bottom horizontals for truss beam for Primary Roof Strucutre, substructure 0
                            _ss.Elements["Line"].Add(sh_hbTrussPrimary);
                        }

                        //vertical lines
                        for (int k = 0; k < sortedtrussPts1.Count(); k++)
                        {
                            Line vLine = new Line(sortedtrussPts1[k], sortedtrussPts2[k]);
                            SH_Node[] vnodes = new SH_Node[2];
                            vnodes[0] = new SH_Node(vLine.From, null);
                            vnodes[1] = new SH_Node(vLine.To, null);
                            SH_Line sh_vTrussPrimary = new SH_Line(vnodes, _ss.elementCount++, "vTrussPrimarySub1"); //verticals for truss beam for Primary Roof Strucutre, substructure 0
                            _ss.Elements["Line"].Add(sh_vTrussPrimary);
                        }

                        //diagonal lines
                        for (int i = 0; i < sortedtrussPts2.Count() - 1; i += 2)
                        {
                            Line dLine = new Line(sortedtrussPts2[i], sortedtrussPts1[i + 1]);
                            SH_Node[] dnodes = new SH_Node[2];
                            dnodes[0] = new SH_Node(dLine.From, null);
                            dnodes[1] = new SH_Node(dLine.To, null);
                            SH_Line sh_dTrussPrimary = new SH_Line(dnodes, _ss.elementCount++, "dTrussPrimarySub1"); //diagonals for truss beam for Primary Roof Strucutre, substructure 0
                            _ss.Elements["Line"].Add(sh_dTrussPrimary);
                        }
                        for (int j = 1; j < sortedtrussPts1.Count() - 1; j += 2)
                        {
                            Line dLine = new Line(sortedtrussPts1[j], sortedtrussPts2[j + 1]);
                            SH_Node[] dnodes = new SH_Node[2];
                            dnodes[0] = new SH_Node(dLine.From, null);
                            dnodes[1] = new SH_Node(dLine.To, null);
                            SH_Line sh_dTrussPrimary = new SH_Line(dnodes, _ss.elementCount++, "dTrussPrimarySub1"); //diagonals for truss beam for Primary Roof Strucutre, substructure 0
                            _ss.Elements["Line"].Add(sh_dTrussPrimary);
                        }

                        //store nodes from sortedtrussPts1 and sortedtrussPts2
                        for (int node1 = 0; node1 < sortedtrussPts1.Count; node1++)
                        {
                            SH_Node[] tnodes = new SH_Node[1];
                            tnodes[0] = new SH_Node(sortedtrussPts1[node1], null);
                            nodeLst.Add(tnodes[0]);
                        }
                        for (int node1 = 0; node1 < sortedtrussPts2.Count; node1++)
                        {
                            SH_Node[] tnodes = new SH_Node[1];
                            tnodes[0] = new SH_Node(sortedtrussPts2[node1], null);
                            nodeLst.Add(tnodes[0]);
                        }
                    }
                    //Remove transversal beams
                    _ss.Elements["Line"].RemoveAll(el => el.elementName == "transBeamSub1");
                }

                if (nrPrimaryRS == 1) // Beam (solid)
                {
                    _ss.name += "_1";                                                    
                }

            }
            // ------------- Primary roof structure for substructure 2 (pitched roof) ------------- 
            if (nrSub[1] == "2")
            {
                // only even numbers
                if (count % 2 == 0)
                {
                    //Search for the transversal beams, bottom beam  
                    var tBeams = from tBeam in _ss.Elements["Line"]
                                 where tBeam.elementName == "transBeamSub2"
                                 select tBeam;

                    //Search for the two pitched beams
                    var pBeams = from pBeam in _ss.Elements["Line"]
                                 where pBeam.elementName == "pitchedRoofSub2"
                                 select pBeam;

                    for (int b = 0; b < 2; b++)
                    {
                        //Bottom beam
                        Point3d[] bPts;
                        SH_Element tb = tBeams.ToList().ElementAt(b);
                        Line bline = new Line(tb.Nodes[0].Position, tb.Nodes[1].Position);
                        Curve bLine = bline.ToNurbsCurve();
                        bLine.DivideByCount(Convert.ToInt32(count), true, out bPts);

                        //Sort the list of points
                        List<Point3d> sortedbPts = new List<Point3d>();
                        if (bLine.PointAtEnd.X == bLine.PointAtStart.X)
                        {
                            sortedbPts = bPts.OrderBy(pt => pt.Y).ToList();
                        }
                        else if (bLine.PointAtEnd.Y == bLine.PointAtStart.Y)
                        {
                            sortedbPts = bPts.OrderBy(pt => pt.X).ToList();
                        }

                        //Inclined beams, pitched beams
                        Point3d[] pPts;
                        List<Point3d> ppts = new List<Point3d>();
                        if (b == 0)
                        {
                            SH_Element pb1 = pBeams.ToList().ElementAt(0);
                            SH_Element pb2 = pBeams.ToList().ElementAt(1);
                            ppts.Add(pb1.Nodes[0].Position);
                            ppts.Add(pb1.Nodes[1].Position);
                            ppts.Add(pb2.Nodes[1].Position);
                        }

                        if (b == 1)
                        {
                            SH_Element pb1 = pBeams.ToList().ElementAt(2);
                            SH_Element pb2 = pBeams.ToList().ElementAt(3);
                            ppts.Add(pb1.Nodes[0].Position);
                            ppts.Add(pb1.Nodes[1].Position);
                            ppts.Add(pb2.Nodes[1].Position);
                        }


                        Curve pcrv = Curve.CreateControlPointCurve(ppts, 1);
                        pcrv.DivideByCount(Convert.ToInt32(count), true, out pPts);

                        //Sort the list of points
                        List<Point3d> sortedpPts = new List<Point3d>();
                        if (pcrv.PointAtEnd.X == pcrv.PointAtStart.X)
                        {
                            sortedpPts = pPts.OrderBy(pt => pt.Y).ToList();
                        }
                        else if (pcrv.PointAtEnd.Y == pcrv.PointAtStart.Y)
                        {
                            sortedpPts = pPts.OrderBy(pt => pt.X).ToList();
                        }

                        //Horizontal bottom lines
                        for (int i = 0; i < sortedbPts.Count() - 1; i++)
                        {
                            Line hbline = new Line(sortedbPts[i], sortedbPts[i + 1]);
                            SH_Node[] hbnodes = new SH_Node[2];
                            hbnodes[0] = new SH_Node(hbline.From, null);
                            hbnodes[1] = new SH_Node(hbline.To, null);
                            SH_Line sh_hbTrussPrimary = new SH_Line(hbnodes, _ss.elementCount++, "hbTrussPrimarySub2"); //bottom horizontals for trusses for Primary Roof Strucutre, substructure 2
                            _ss.Elements["Line"].Add(sh_hbTrussPrimary);
                        }

                        //Vertical lines
                        for (int n = 1; n < sortedbPts.Count() - 1; n++)
                        {
                            Line vline = new Line(sortedbPts[n], sortedpPts[n]);
                            SH_Node[] vnodes = new SH_Node[2];
                            vnodes[0] = new SH_Node(vline.From, null);
                            vnodes[1] = new SH_Node(vline.To, null);
                            SH_Line sh_vTrussPrimary = new SH_Line(vnodes, _ss.elementCount++, "vTrussPrimarySub2"); //verticals for trusses for Primary Roof Strucutre, substructure 2
                            _ss.Elements["Line"].Add(sh_vTrussPrimary);
                        }

                        //Pitched lines
                        for (int j = 0; j < sortedpPts.Count() - 1; j++)
                        {
                            Line pLine = new Line(sortedpPts[j], sortedpPts[j + 1]);
                            SH_Node[] pnodes = new SH_Node[2];
                            pnodes[0] = new SH_Node(pLine.From, null);
                            pnodes[1] = new SH_Node(pLine.To, null);
                            SH_Line sh_pTrussPrimary = new SH_Line(pnodes, _ss.elementCount++, "pTrussPrimarySub2"); //inclined lines for trusses for Primary Roof Strucutre, substructure 2
                            _ss.Elements["Line"].Add(sh_pTrussPrimary);
                        }

                        //Trusses/diagonals
                        for (int k = 1; k < sortedbPts.Count() / 2; k++)
                        {
                            Line line = new Line(sortedpPts[k], sortedbPts[k + 1]);
                            SH_Node[] nodes = new SH_Node[2];
                            nodes[0] = new SH_Node(line.From, null);
                            nodes[1] = new SH_Node(line.To, null);
                            SH_Line sh_dTrussPrimary = new SH_Line(nodes, _ss.elementCount++, "dTrussPrimarySub2"); //diagonals for trusses for Primary Roof Strucutre, substructure 2
                            _ss.Elements["Line"].Add(sh_dTrussPrimary);
                        }
                        for (int m = sortedbPts.Count() / 2; m < sortedbPts.Count() - 2; m++)
                        {
                            Line line = new Line(sortedbPts[m], sortedpPts[m + 1]);
                            SH_Node[] nodes = new SH_Node[2];
                            nodes[0] = new SH_Node(line.From, null);
                            nodes[1] = new SH_Node(line.To, null);
                            SH_Line sh_dTrussPrimary = new SH_Line(nodes, _ss.elementCount++, "dTrussPrimarySub2"); //diagonals for trusses for Primary Roof Strucutre, substructure 2
                            _ss.Elements["Line"].Add(sh_dTrussPrimary);
                        }

                        //store nodes from bPts and pPts
                        for (int node1 = 0; node1 < sortedbPts.Count; node1++)
                        {
                            SH_Node[] tnodes = new SH_Node[1];
                            tnodes[0] = new SH_Node(sortedbPts[node1], null);
                            nodeLst.Add(tnodes[0]);
                        }
                        for (int node1 = 0; node1 < sortedpPts.Count; node1++)
                        {
                            SH_Node[] tnodes = new SH_Node[1];
                            tnodes[0] = new SH_Node(sortedpPts[node1], null);
                            nodeLst.Add(tnodes[0]);
                        }
                    }
                }
                //Remove transversal and pitched beams
                _ss.Elements["Line"].RemoveAll(el => el.elementName == "transBeamSub2");
               
            }
            // ------------- Primary roof structure for substructure 3 (bowed roof) ------------- 
            if (nrSub[1] == "3")
            {
                //Search for the transversal beams, bottom beam  
                var tBeams = from tBeam in _ss.Elements["Line"]
                             where tBeam.elementName == "transBeamSub3"
                             select tBeam;

                //Search for the two bowed beams
                var bBeams = from bBeam in _ss.Elements["Line"]
                             where bBeam.elementName == "bowedRoofSub3"
                             select bBeam;

                for (int s = 0; s < tBeams.Count(); s++)
                {
                    //bottom beam
                    Point3d[] bPts;
                    List<Point3d> bpts = new List<Point3d>();
                    SH_Element tb = tBeams.ToList().ElementAt(s);
                    Line bline = new Line(tb.Nodes[0].Position, tb.Nodes[1].Position); //construct bottom beam 
                    Curve bLine = bline.ToNurbsCurve();

                    //Get half of "bowedRoofSub3" elements (since it represent two arches)
                    List<SH_Element> bowedLst1 = new List<SH_Element>(); ;
                    for (int i = 0; i < bBeams.Count() / 2; i++)
                    {
                        SH_Element el = bBeams.ElementAt(i);
                        bowedLst1.Add(el);
                    }
                    List<SH_Element> bowedLst2 = new List<SH_Element>(); ;
                    for (int i = bBeams.Count() / 2; i < bBeams.Count(); i++)
                    {
                        SH_Element el = bBeams.ElementAt(i);
                        bowedLst2.Add(el);
                    }

                    List<SH_Element> bowedLst = new List<SH_Element>(); ;
                    if (s == 0)
                    {
                        bowedLst = bowedLst1;
                    }
                    else if (s == 1)
                    {
                        bowedLst = bowedLst2;
                    }


                    var cnt = bowedLst.Count(); //number of segments of one arch
                                                //bLine.DivideByCount(cnt - 1, true, out bPts); //one less segment than the arch, for another truss type
                    bLine.DivideByCount(cnt, true, out bPts);
                    //Sort the list of points
                    List<Point3d> sortedbPts = new List<Point3d>();
                    if (bLine.PointAtEnd.X == bLine.PointAtStart.X)
                    {
                        sortedbPts = bPts.OrderBy(pt => pt.Y).ToList();
                    }
                    else if (bLine.PointAtEnd.Y == bLine.PointAtStart.Y)
                    {
                        sortedbPts = bPts.OrderBy(pt => pt.X).ToList();
                    }

                    //store nodes from bPts1, end points are already stored
                    for (int node1 = 0; node1 < sortedbPts.Count - 1; node1++)
                    {
                        SH_Node[] tnodes = new SH_Node[1];
                        tnodes[0] = new SH_Node(sortedbPts[node1], null);
                        nodeLst.Add(tnodes[0]);
                    }

                    for (int b = 0; b < sortedbPts.Count() - 1; b++)
                    {
                        Line botline = new Line(sortedbPts[b], sortedbPts[b + 1]);
                        SH_Node[] bnodes = new SH_Node[2];
                        bnodes[0] = new SH_Node(botline.From, null);
                        bnodes[1] = new SH_Node(botline.To, null);
                        SH_Line sh_dTrussPrimary = new SH_Line(bnodes, _ss.elementCount++, "bTrussPrimarySub3"); //bottom beams for Primary Roof Strucutre, substructure 3
                        _ss.Elements["Line"].Add(sh_dTrussPrimary);
                    }

                    //Arch
                    //Sort the list of elements, the axis of the transversal beam are the same for the arch
                    List<SH_Element> bowedBeams = new List<SH_Element>();
                    if (bLine.PointAtEnd.X == bLine.PointAtStart.X)
                    {
                        bowedBeams = bowedLst.OrderBy(bb => bb.Nodes[0].Position.Y).ToList();
                    }
                    else if (bLine.PointAtEnd.Y == bLine.PointAtStart.Y)
                    {
                        bowedBeams = bowedLst.OrderBy(bb => bb.Nodes[0].Position.X).ToList();
                    }

                    //Trusses, diagonals
                    for (int bb1 = 1; bb1 < bowedLst.Count() / 2; bb1++)
                    {
                        Line line = new Line(bowedBeams.ElementAt(bb1).Nodes[0].Position, sortedbPts[bb1 + 1]);
                        SH_Node[] bnodes = new SH_Node[2];
                        bnodes[0] = new SH_Node(line.From, null);
                        bnodes[1] = new SH_Node(line.To, null);
                        SH_Line sh_dTrussPrimary = new SH_Line(bnodes, _ss.elementCount++, "dTrussPrimarySub3"); //trusses for Primary Roof Strucutre, substructure 3
                        _ss.Elements["Line"].Add(sh_dTrussPrimary);
                    }
                    for (int bb2 = bowedLst.Count() / 2; bb2 < bowedLst.Count() - 1; bb2++)
                    {
                        Line line = new Line(sortedbPts[bb2], bowedBeams.ElementAt(bb2 + 1).Nodes[0].Position);
                        SH_Node[] bnodes = new SH_Node[2];
                        bnodes[0] = new SH_Node(line.From, null);
                        bnodes[1] = new SH_Node(line.To, null);
                        SH_Line sh_dTrussPrimary = new SH_Line(bnodes, _ss.elementCount++, "dTrussPrimarySub3"); //trusses for Primary Roof Strucutre, substructure 3
                        _ss.Elements["Line"].Add(sh_dTrussPrimary);
                    }
                    /*for (int bb1 = 1; bb1 < bowedLst.Count()-1; bb1++)
                    {
                        Line line = new Line(bowedBeams.ElementAt(bb1).Nodes[0].Position, sortedbPts[bb1]);
                        SH_Node[] bnodes = new SH_Node[2];
                        bnodes[0] = new SH_Node(line.From, null);
                        bnodes[1] = new SH_Node(line.To, null);
                        SH_Line sh_dTrussPrimary = new SH_Line(bnodes, _ss.elementCount++, "dTrussPrimarySub3"); //trusses for Primary Roof Strucutre, substructure 3
                        _ss.Elements["Line"].Add(sh_dTrussPrimary);
                    }
                    for (int bb2 = 1; bb2 < bowedLst.Count()-1; bb2++)
                    {
                        Line line = new Line(sortedbPts[bb2], bowedBeams.ElementAt(bb2 + 1).Nodes[0].Position);
                        SH_Node[] bnodes = new SH_Node[2];
                        bnodes[0] = new SH_Node(line.From, null);
                        bnodes[1] = new SH_Node(line.To, null);
                        SH_Line sh_dTrussPrimary = new SH_Line(bnodes, _ss.elementCount++, "dTrussPrimarySub3"); //trusses for Primary Roof Strucutre, substructure 3
                        _ss.Elements["Line"].Add(sh_dTrussPrimary);
                    }*/
                    //Remove transversal and pitched beams

                    //vertical lines
                    for (int n = 1; n < sortedbPts.Count() - 1; n++)
                    {
                        Line vline = new Line(sortedbPts[n], bowedBeams.ElementAt(n).Nodes[0].Position);
                        SH_Node[] vnodes = new SH_Node[2];
                        vnodes[0] = new SH_Node(vline.From, null);
                        vnodes[1] = new SH_Node(vline.To, null);
                        SH_Line sh_vTrussPrimary = new SH_Line(vnodes, _ss.elementCount++, "vTrussPrimarySub3"); //verticals for trusses for Primary Roof Strucutre, substructure 2
                        _ss.Elements["Line"].Add(sh_vTrussPrimary);
                    }
                }
                    _ss.Elements["Line"].RemoveAll(el => el.elementName == "transBeamSub3");
            }

            //Add all nodes to SH_Elements
            _ss.Nodes = new List<SH_Node>();
            _ss.Nodes.AddRange(nodeLst);

            // change the state
            _ss.SimpleShapeState = State.delta;
            return "PrimaryRoofStructure successfully applied.";
        }

        public override State GetNextState()
        {
            return State.delta;
        }

    }
}
