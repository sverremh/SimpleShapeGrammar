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
    public class SubStructureRule : SH_Rule
    {
        // --- properties ---
        //public List<string> nameSubLSt = new List<string>();
        public double NrSub; //number of substructure
        public double H; // height for pitched and bowed roof
        public double Count; //counts to divide arch (bowed roof)
        public string cSec; // Cross section name
        SH_Material beamMat; // Material


        // --- constructor --- 
        public SubStructureRule()
        {
            Name = "SubStructureClass";
            RuleState = State.beta;
        }

        public SubStructureRule(double _nrSub, double _h, double _count, string _cSec, SH_Material _beamMat)
        {
            NrSub = _nrSub;
            H = _h;
            Count = _count;
            cSec = _cSec;
            beamMat = _beamMat;
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
                return "The State is not compatible with SubStructureRule.";
            }

            _ss.name = "Mitchell";
            if (Count % 2 == 1)
            {
                Count = Count + 1;
            }

            // ---------------------- SUBSTRUCTURE 0 --------------------
            if (NrSub == 0)
            {
                _ss.name += "_0";

                List<Curve> col = new List<Curve>(); //store columns before giving id
                List<SH_Node> nodes = new List<SH_Node>(); //list for nodes
                _ss.Elements["Line"] = new List<SH_Element>(); //empty list
                List<SH_Element> elementLst = _ss.Elements["Surface"];

                foreach (SH_Element element in elementLst)
                {
                    SH_Surface sh_srf = (SH_Surface)element;
                    string n = sh_srf.elementName;

                    if (n == "Top")
                    {
                        Surface s = sh_srf.elementSurface;
                        //find which side is the longest, and if they are equal
                        if (s.IsoCurve(0, 0).GetLength() > s.IsoCurve(1, 1).GetLength() || s.IsoCurve(0, 0).GetLength() == s.IsoCurve(1, 1).GetLength())
                        {

                            Curve longBeam1 = s.IsoCurve(0, 0); //longitudinal beam = longest edge of surface
                            Curve longBeam2 = s.IsoCurve(0, 1);

                            SH_Node[] lnodes1 = new SH_Node[2];
                            lnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            lnodes1[1] = new SH_Node(longBeam1.PointAtEnd, null);
                            SH_Node[] lnodes2 = new SH_Node[2];
                            lnodes2[0] = new SH_Node(longBeam2.PointAtStart, null);
                            lnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);
                            SH_Node[] tnodes1 = new SH_Node[2];
                            tnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            tnodes1[1] = new SH_Node(longBeam2.PointAtStart, null);
                            SH_Node[] tnodes2 = new SH_Node[2];
                            tnodes2[0] = new SH_Node(longBeam1.PointAtEnd, null);
                            tnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);

                            //SH_Line
                            SH_Line sh_longBeam1 = new SH_Line(lnodes1, _ss.elementCount++, "longBeam_Mitchell_0"); //longitudinal beam 1
                            sh_longBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                            _ss.Elements["Line"].Add(sh_longBeam1);
                            SH_Line sh_longBeam2 = new SH_Line(lnodes2, _ss.elementCount++, "longBeam_Mitchell_0"); //longitudinal beam 2 
                            sh_longBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element    
                            _ss.Elements["Line"].Add(sh_longBeam2);
                            SH_Line sh_transBeam1 = new SH_Line(tnodes1, _ss.elementCount++, "transBeam_Mitchell_0"); //transversal beam 1
                            sh_transBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                            _ss.Elements["Line"].Add(sh_transBeam1);
                            SH_Line sh_transBeam2 = new SH_Line(tnodes2, _ss.elementCount++, "transBeam_Mitchell_0"); //transversal beam 2
                            sh_transBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element    
                            _ss.Elements["Line"].Add(sh_transBeam2);

                            //store top nodes
                            nodes.Add(lnodes1[0]);
                            nodes.Add(lnodes1[1]);
                            nodes.Add(lnodes2[0]);
                            nodes.Add(lnodes2[1]);

                            //Curve[] c11 = c1.Offset(Plane.WorldXY, t, 000.1, 0);
                            //Curve[] c22 = c2.Offset(Plane.WorldXY, -t, 000.1, 0);

                            //Brep[] srf = Brep.CreateDevelopableLoft(c11[0], c22[0], false, false, 1);
                            //Brep tbrep = Brep.CreateFromOffsetFace(srf[0].Faces[0], t / 2, 0.0001, true, true);
                            //SH_Solid sh_brep = new SH_Solid("Top0", tbrep);
                            //_ss.Elements["Solid"].Add(sh_brep);
                        }
                        else if (s.IsoCurve(0, 0).GetLength() < s.IsoCurve(1, 1).GetLength())
                        {
                            Curve longBeam1 = s.IsoCurve(1, 1); //longitudinal beam = longest edge of surface
                            Curve longBeam2 = s.IsoCurve(1, 0);

                            SH_Node[] lnodes1 = new SH_Node[2];
                            lnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            lnodes1[1] = new SH_Node(longBeam1.PointAtEnd, null);
                            SH_Node[] lnodes2 = new SH_Node[2];
                            lnodes2[0] = new SH_Node(longBeam2.PointAtStart, null);
                            lnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);
                            SH_Node[] tnodes1 = new SH_Node[2];
                            tnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            tnodes1[1] = new SH_Node(longBeam2.PointAtStart, null);
                            SH_Node[] tnodes2 = new SH_Node[2];
                            tnodes2[0] = new SH_Node(longBeam1.PointAtEnd, null);
                            tnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);

                            //SH_Line
                            SH_Line sh_longBeam1 = new SH_Line(lnodes1, _ss.elementCount++, "longBeam_Mitchell_0"); //longitudinal beam 1
                            sh_longBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                            _ss.Elements["Line"].Add(sh_longBeam1);
                            SH_Line sh_longBeam2 = new SH_Line(lnodes2, _ss.elementCount++, "longBeam_Mitchell_0"); //longitudinal beam 2 
                            sh_longBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_longBeam2);
                            SH_Line sh_transBeam1 = new SH_Line(tnodes1, _ss.elementCount++, "transBeam_Mitchell_0"); //transversal beam 1
                            sh_transBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                            _ss.Elements["Line"].Add(sh_transBeam1);
                            SH_Line sh_transBeam2 = new SH_Line(tnodes2, _ss.elementCount++, "transBeam_Mitchell_0"); //transversal beam 2
                            sh_transBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element   
                            _ss.Elements["Line"].Add(sh_transBeam2);

                            //store top nodes
                            nodes.Add(lnodes1[0]);
                            nodes.Add(lnodes1[1]);
                            nodes.Add(lnodes2[0]);
                            nodes.Add(lnodes2[1]);
                        }
                    }

                    else if (n == "Shortest Wall")
                    {
                        Surface s = sh_srf.elementSurface;

                        //Find vertical curves of a surface = columns
                        Curve crv1 = s.IsoCurve(0, 0);
                        Vector3d vec1 = crv1.TangentAt(0.5);
                        Curve crv2 = s.IsoCurve(1, 1);
                        Vector3d vec2 = crv2.TangentAt(0.5);

                        //vertical curves = column
                        if (vec1.Z > 0)
                        {
                            //vertical curves = column
                            Curve c1 = s.IsoCurve(0, 0);
                            Curve c2 = s.IsoCurve(0, 1);
                            col.Add(c1);
                            col.Add(c2);
                        }
                        else if (vec2.Z > 0)
                        {
                            //vertical curves = column
                            Curve c1 = s.IsoCurve(1, 0);
                            Curve c2 = s.IsoCurve(1, 1);
                            col.Add(c1);
                            col.Add(c2);
                        }
                    }
                }
                //columns and nodes
                for (int j = 0; j < col.Count; j++)
                {
                    Curve c = col[j];
                    if (c.PointAtStart.Z > c.PointAtEnd.Z) //wants the node with the lowest Z value
                    {
                        SH_Node[] cnodes = new SH_Node[2];
                        cnodes[0] = new SH_Node(c.PointAtStart, null);
                        cnodes[1] = new SH_Node(c.PointAtEnd, null);

                        nodes.Add(cnodes[1]); //add end node to node list

                        SH_Line sh_col = new SH_Line(cnodes, _ss.elementCount++, "Column");
                        sh_col.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element     
                        _ss.Elements["Line"].Add(sh_col);
                    }

                    else if (c.PointAtStart.Z < c.PointAtEnd.Z) //wants the node with the lowest Z value
                    {
                        SH_Node[] cnodes = new SH_Node[2];
                        cnodes[0] = new SH_Node(c.PointAtStart, null);
                        cnodes[1] = new SH_Node(c.PointAtEnd, null);

                        nodes.Add(cnodes[0]); //add end node to node list

                        SH_Line sh_col = new SH_Line(cnodes, _ss.elementCount++, "Column");
                        sh_col.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element     
                        _ss.Elements["Line"].Add(sh_col);
                    }
                }
                //Add nodes to Simple Shape
                _ss.Nodes = new List<SH_Node>();
                _ss.Nodes.AddRange(nodes);
            }


            // -------------------- SUBSTRUCTOR nr 1 --------------------
            if (NrSub == 1)
            {
                _ss.name += "_1";
                List<Curve> col = new List<Curve>(); //store columns before giving id
                List<SH_Node> nodes = new List<SH_Node>(); //list for nodes
                _ss.Elements["Line"] = new List<SH_Element>(); //empty list
                List<SH_Element> elementLst = _ss.Elements["Surface"];

                foreach (SH_Element element in elementLst)
                {
                    SH_Surface sh_srf = (SH_Surface)element;
                    string n = sh_srf.elementName;

                    // add thickness to elements
                    if (n == "Top")
                    {
                        Surface s = sh_srf.elementSurface;
                        //find which side is the longest
                        if (s.IsoCurve(0, 0).GetLength() > s.IsoCurve(1, 1).GetLength() || s.IsoCurve(0, 0).GetLength() == s.IsoCurve(1, 1).GetLength())
                        {
                            Curve longBeam1 = s.IsoCurve(0, 0); //longitudinal beam = longest edge of surface
                            Curve longBeam2 = s.IsoCurve(0, 1);

                            SH_Node[] lnodes1 = new SH_Node[2];
                            lnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            lnodes1[1] = new SH_Node(longBeam1.PointAtEnd, null);
                            SH_Node[] lnodes2 = new SH_Node[2];
                            lnodes2[0] = new SH_Node(longBeam2.PointAtStart, null);
                            lnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);
                            SH_Node[] tnodes1 = new SH_Node[2];
                            tnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            tnodes1[1] = new SH_Node(longBeam2.PointAtStart, null);
                            SH_Node[] tnodes2 = new SH_Node[2];
                            tnodes2[0] = new SH_Node(longBeam1.PointAtEnd, null);
                            tnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);

                            //SH_Line
                            SH_Line sh_longBeam1 = new SH_Line(lnodes1, _ss.elementCount++, "longBeam_Mitchell_1"); //longitudinal beam 1
                            sh_longBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_longBeam1);
                            SH_Line sh_longBeam2 = new SH_Line(lnodes2, _ss.elementCount++, "longBeam_Mitchell_1"); //longitudinal beam 2 
                            sh_longBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_longBeam2);
                            SH_Line sh_transBeam1 = new SH_Line(tnodes1, _ss.elementCount++, "transBeam_Mitchell_1"); //transversal beam 1
                            sh_transBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_transBeam1);
                            SH_Line sh_transBeam2 = new SH_Line(tnodes2, _ss.elementCount++, "transBeam_Mitchell_1"); //transversal beam 2
                            sh_transBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_transBeam2);

                            //store top nodes
                            nodes.Add(lnodes1[0]);
                            nodes.Add(lnodes1[1]);
                            nodes.Add(lnodes2[0]);
                            nodes.Add(lnodes2[1]);
                        }

                        else if (s.IsoCurve(0, 0).GetLength() < s.IsoCurve(1, 1).GetLength())
                        {
                            Curve longBeam1 = s.IsoCurve(1, 1); //longitudinal beam = longest edge of surface
                            Curve longBeam2 = s.IsoCurve(1, 0);

                            SH_Node[] lnodes1 = new SH_Node[2];
                            lnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            lnodes1[1] = new SH_Node(longBeam1.PointAtEnd, null);
                            SH_Node[] lnodes2 = new SH_Node[2];
                            lnodes2[0] = new SH_Node(longBeam2.PointAtStart, null);
                            lnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);
                            SH_Node[] tnodes1 = new SH_Node[2];
                            tnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            tnodes1[1] = new SH_Node(longBeam2.PointAtStart, null);
                            SH_Node[] tnodes2 = new SH_Node[2];
                            tnodes2[0] = new SH_Node(longBeam1.PointAtEnd, null);
                            tnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);

                            //SH_Line
                            SH_Line sh_longBeam1 = new SH_Line(lnodes1, _ss.elementCount++, "longBeam_Mitchell_1"); //longitudinal beam 1
                            sh_longBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_longBeam1);
                            SH_Line sh_longBeam2 = new SH_Line(lnodes2, _ss.elementCount++, "longBeam_Mitchell_1"); //longitudinal beam 2 
                            sh_longBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_longBeam2);
                            SH_Line sh_transBeam1 = new SH_Line(tnodes1, _ss.elementCount++, "transBeam_Mitchell_1"); //transversal beam 1
                            sh_transBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_transBeam1);
                            SH_Line sh_transBeam2 = new SH_Line(tnodes2, _ss.elementCount++, "transBeam_Mitchell_1"); //transversal beam 2
                            sh_transBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_transBeam2);

                            //add top nodes
                            nodes.Add(lnodes1[0]);
                            nodes.Add(lnodes1[1]);
                            nodes.Add(lnodes2[0]);
                            nodes.Add(lnodes2[1]);
                        }
                    }

                    else if (n == "Shortest Wall")
                    {
                        Surface s = sh_srf.elementSurface;

                        //Find vertical curves of a surface = columns
                        Curve crv1 = s.IsoCurve(0, 0);
                        Vector3d vec1 = crv1.TangentAt(0.5);
                        Curve crv2 = s.IsoCurve(1, 1);
                        Vector3d vec2 = crv2.TangentAt(0.5);

                        //vertical curves = columns
                        if (vec1.Z > 0)
                        {
                            Curve c1 = s.IsoCurve(0, 0);
                            Curve c2 = s.IsoCurve(0, 1);
                            col.Add(c1);
                            col.Add(c2);
                        }
                        else if (vec2.Z > 0)
                        {
                            Curve c1 = s.IsoCurve(1, 0);
                            Curve c2 = s.IsoCurve(1, 1);
                            col.Add(c1);
                            col.Add(c2);
                        }
                    }
                }
                //columns and nodes
                for (int j = 0; j < col.Count; j++)
                {
                    Curve c = col[j];
                    if (c.PointAtStart.Z > c.PointAtEnd.Z) //want the node with the lowest Z value (bottom node)
                    {
                        SH_Node[] cnodes = new SH_Node[2];
                        cnodes[0] = new SH_Node(c.PointAtStart, null);
                        cnodes[1] = new SH_Node(c.PointAtEnd, null);

                        nodes.Add(cnodes[1]); //add bottom node to node list

                        SH_Line sh_col = new SH_Line(cnodes, _ss.elementCount++, "Column");
                        sh_col.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                        _ss.Elements["Line"].Add(sh_col);
                    }

                    else if (c.PointAtStart.Z < c.PointAtEnd.Z) //want the node with the lowest Z value (bottom node)
                    {
                        SH_Node[] cnodes = new SH_Node[2];
                        cnodes[0] = new SH_Node(c.PointAtStart, null);
                        cnodes[1] = new SH_Node(c.PointAtEnd, null);

                        nodes.Add(cnodes[0]); //add bottom node to node list

                        SH_Line sh_col = new SH_Line(cnodes, _ss.elementCount++, "Column");
                        sh_col.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                        _ss.Elements["Line"].Add(sh_col);
                    }
                }

                //Add all nodes to Simple Shape
                _ss.Nodes = new List<SH_Node>();
                _ss.Nodes.AddRange(nodes);
            }

            // ---------------------- SUBSTRUCTOR nr 2 ------------------- (pitched roof)
            if (NrSub == 2)
            {
                _ss.name += "_2";
                List<Curve> col = new List<Curve>(); //store columns before giving id
                List<SH_Node> nodes = new List<SH_Node>(); //list for nodes
                _ss.Elements["Line"] = new List<SH_Element>(); //empty list
                List<SH_Element> elementLst = _ss.Elements["Surface"];

                foreach (SH_Element element in elementLst)
                {
                    SH_Surface sh_srf = (SH_Surface)element;
                    string n = sh_srf.elementName;

                    if (n == "Top")
                    {
                        Surface s = sh_srf.elementSurface;
                        //find which side is the longest, trim curve, and offsets the surface
                        if (s.IsoCurve(0, 0).GetLength() > s.IsoCurve(1, 1).GetLength() || s.IsoCurve(0, 0).GetLength() == s.IsoCurve(1, 1).GetLength())
                        {
                            Curve longBeam1 = s.IsoCurve(0, 0);
                            Curve longBeam2 = s.IsoCurve(0, 1);

                            SH_Node[] lnodes1 = new SH_Node[2];
                            lnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            lnodes1[1] = new SH_Node(longBeam1.PointAtEnd, null);
                            SH_Node[] lnodes2 = new SH_Node[2];
                            lnodes2[0] = new SH_Node(longBeam2.PointAtStart, null);
                            lnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);
                            SH_Node[] tnodes1 = new SH_Node[2];
                            tnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            tnodes1[1] = new SH_Node(longBeam2.PointAtStart, null);
                            SH_Node[] tnodes2 = new SH_Node[2];
                            tnodes2[0] = new SH_Node(longBeam1.PointAtEnd, null);
                            tnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);

                            SH_Line sh_longBeam1 = new SH_Line(lnodes1, _ss.elementCount++, "longBeam_Mitchell_2"); //longitudinal beam 1
                            sh_longBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_longBeam1);
                            SH_Line sh_longBeam2 = new SH_Line(lnodes2, _ss.elementCount++, "longBeam_Mitchell_2"); //longitudinal beam 2 
                            sh_longBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_longBeam2);
                            SH_Line sh_transBeam1 = new SH_Line(tnodes1, _ss.elementCount++, "transBeam_Mitchell_2"); //transversal beam 1
                            sh_transBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_transBeam1);
                            SH_Line sh_transBeam2 = new SH_Line(tnodes2, _ss.elementCount++, "transBeam_Mitchell_2"); //transversal beam 2
                            sh_transBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_transBeam2);

                            //add top nodes to list
                            nodes.Add(lnodes1[0]);
                            nodes.Add(lnodes1[1]);
                            nodes.Add(lnodes2[0]);
                            nodes.Add(lnodes2[1]);

                            // Create pitched roof
                            // Create the picthed roof at two transversal sides
                            Curve transBeam1 = s.IsoCurve(1, 0);
                            List<Point3d> pts1 = new List<Point3d>();
                            Point3d sPt1 = transBeam1.PointAtStart;
                            pts1.Add(sPt1);
                            Point3d midPt1 = transBeam1.PointAtNormalizedLength(0.5);
                            Point3d transMidPt1 = new Point3d(midPt1.X, midPt1.Y, midPt1.Z + H); //move midpoint in z-dir with a distance h
                            pts1.Add(transMidPt1);
                            Point3d ePt1 = transBeam1.PointAtEnd;
                            pts1.Add(ePt1);

                            for (int i = 0; i < pts1.Count - 1; i++)
                            {
                                Line pRoof1 = new Line(pts1[i], pts1[i + 1]);
                                SH_Node[] pnodes1 = new SH_Node[2];
                                pnodes1[0] = new SH_Node(pRoof1.From, null);
                                pnodes1[1] = new SH_Node(pRoof1.To, null);

                                SH_Line sh_pitchedRoof = new SH_Line(pnodes1, _ss.elementCount++, "pitchedBeam_Mitchell_2"); // pitched roof at one side
                                sh_pitchedRoof.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_pitchedRoof);
                            }

                            Curve transBeam2 = s.IsoCurve(1, 1);
                            List<Point3d> pts2 = new List<Point3d>();
                            Point3d sPt2 = transBeam2.PointAtStart;
                            pts2.Add(sPt2);
                            Point3d midPt2 = transBeam2.PointAtNormalizedLength(0.5);
                            Point3d transMidPt2 = new Point3d(midPt2.X, midPt2.Y, midPt2.Z + H); //move midpoint in z-dir with a distance h
                            pts2.Add(transMidPt2);
                            Point3d ePt2 = transBeam2.PointAtEnd;
                            pts2.Add(ePt2);

                            for (int j = 0; j < pts2.Count - 1; j++)
                            {
                                Line pRoof2 = new Line(pts2[j], pts2[j + 1]);
                                SH_Node[] pnodes2 = new SH_Node[2];
                                pnodes2[0] = new SH_Node(pRoof2.From, null);
                                pnodes2[1] = new SH_Node(pRoof2.To, null);

                                SH_Line sh_pitchedRoof = new SH_Line(pnodes2, _ss.elementCount++, "pitchedBeam_Mitchell_2"); // pitched roof at one side
                                sh_pitchedRoof.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element
                                _ss.Elements["Line"].Add(sh_pitchedRoof);
                            }

                            //store nodes from pitched roof
                            nodes.Add(new SH_Node(midPt1, null)); //top node, pitched roof
                            nodes.Add(new SH_Node(midPt2, null)); //top node, pitched roof
                        }

                        else if (s.IsoCurve(0, 0).GetLength() < s.IsoCurve(1, 1).GetLength())
                        {
                            Curve longBeam1 = s.IsoCurve(1, 1); //longitudinal beam = longest beam
                            Curve longBeam2 = s.IsoCurve(1, 0);

                            SH_Node[] lnodes1 = new SH_Node[2];
                            lnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            lnodes1[1] = new SH_Node(longBeam1.PointAtEnd, null);
                            SH_Node[] lnodes2 = new SH_Node[2];
                            lnodes2[0] = new SH_Node(longBeam2.PointAtStart, null);
                            lnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);
                            SH_Node[] tnodes1 = new SH_Node[2];
                            tnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            tnodes1[1] = new SH_Node(longBeam2.PointAtStart, null);
                            SH_Node[] tnodes2 = new SH_Node[2];
                            tnodes2[0] = new SH_Node(longBeam1.PointAtEnd, null);
                            tnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);

                            //SH_Line
                            SH_Line sh_longBeam1 = new SH_Line(lnodes1, _ss.elementCount++, "longBeam_Mitchell_2"); //longitudinal beam 1
                            sh_longBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_longBeam1);
                            SH_Line sh_longBeam2 = new SH_Line(lnodes2, _ss.elementCount++, "longBeam_Mitchell_2"); //longitudinal beam 2 
                            sh_longBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_longBeam2);
                            SH_Line sh_transBeam1 = new SH_Line(tnodes1, _ss.elementCount++, "transBeam_Mitchell_2"); //transversal beam 1
                            sh_transBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_transBeam1);
                            SH_Line sh_transBeam2 = new SH_Line(tnodes2, _ss.elementCount++, "transBeam_Mitchell_2"); //transversal beam 2
                            sh_transBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_transBeam2);

                            //store top nodes
                            nodes.Add(lnodes1[0]);
                            nodes.Add(lnodes1[1]);
                            nodes.Add(lnodes2[0]);
                            nodes.Add(lnodes2[1]);

                            // Pitched roof at the one transversal end
                            Curve transBeam1 = s.IsoCurve(0, 0);
                            List<Point3d> pts1 = new List<Point3d>();
                            Point3d sPt1 = transBeam1.PointAtStart;
                            pts1.Add(sPt1);
                            Point3d midPt1 = transBeam1.PointAtNormalizedLength(0.5);
                            Point3d transMidPt1 = new Point3d(midPt1.X, midPt1.Y, midPt1.Z + H); //move midpoint in z-dir with a distance h
                            pts1.Add(transMidPt1);
                            Point3d ePt1 = transBeam1.PointAtEnd;
                            pts1.Add(ePt1);

                            for (int i = 0; i < pts1.Count - 1; i++)
                            {
                                Line pRoof1 = new Line(pts1[i], pts1[i + 1]);
                                SH_Node[] pnodes1 = new SH_Node[2];
                                pnodes1[0] = new SH_Node(pRoof1.From, null);
                                pnodes1[1] = new SH_Node(pRoof1.To, null);

                                SH_Line sh_pitchedRoof = new SH_Line(pnodes1, _ss.elementCount++, "pitchedBeam_Mitchell_2"); // pitched roof at one side
                                sh_pitchedRoof.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                                _ss.Elements["Line"].Add(sh_pitchedRoof);
                            }

                            // Pitched roof at the other transversal end
                            Curve transBeam2 = s.IsoCurve(0, 1);
                            List<Point3d> pts2 = new List<Point3d>();
                            Point3d sPt2 = transBeam2.PointAtStart;
                            pts2.Add(sPt2);
                            Point3d midPt2 = transBeam2.PointAtNormalizedLength(0.5);
                            Point3d transMidPt2 = new Point3d(midPt2.X, midPt2.Y, midPt2.Z + H); //move midpoint in z-dir with a distance h
                            pts2.Add(transMidPt2);
                            Point3d ePt2 = transBeam2.PointAtEnd;
                            pts2.Add(ePt2);

                            for (int j = 0; j < pts2.Count - 1; j++)
                            {
                                Line pRoof2 = new Line(pts2[j], pts2[j + 1]);
                                SH_Node[] pnodes2 = new SH_Node[2];
                                pnodes2[0] = new SH_Node(pRoof2.From, null);
                                pnodes2[1] = new SH_Node(pRoof2.To, null);

                                SH_Line sh_pitchedRoof = new SH_Line(pnodes2, _ss.elementCount++, "pitchedBeam_Mitchell_2"); // pitched roof at one side
                                sh_pitchedRoof.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                                _ss.Elements["Line"].Add(sh_pitchedRoof);
                            }

                            //store nodes from pitched roof
                            nodes.Add(new SH_Node(midPt1, null)); //top node, pitched roof
                            nodes.Add(new SH_Node(midPt2, null));  //top node, pitched roof
                        }
                    }

                    //Find Columns + nodes
                    else if (n == "Shortest Wall")
                    {
                        Surface s = sh_srf.elementSurface;

                        //Find vertical curves of a surface = columns
                        Curve crv1 = s.IsoCurve(0, 0);
                        Vector3d vec1 = crv1.TangentAt(0.5);
                        Curve crv2 = s.IsoCurve(1, 1);
                        Vector3d vec2 = crv2.TangentAt(0.5);

                        //vertical curves = column
                        if (vec1.Z > 0)
                        {
                            Curve c1 = s.IsoCurve(0, 0);
                            Curve c2 = s.IsoCurve(0, 1);
                            col.Add(c1);
                            col.Add(c2);

                        }
                        else if (vec2.Z > 0)
                        {
                            Curve c1 = s.IsoCurve(1, 0);
                            Curve c2 = s.IsoCurve(1, 1);
                            col.Add(c1);
                            col.Add(c2);
                        }
                    }
                }
                // Give the nodes id and add the columns to the SH_Element
                for (int j = 0; j < col.Count; j++)
                {
                    Curve c = col[j];
                    if (c.PointAtStart.Z > c.PointAtEnd.Z) //wants the node with the lowest Z value (bottom node)
                    {
                        SH_Node[] cnodes = new SH_Node[2];
                        cnodes[0] = new SH_Node(c.PointAtStart, null);
                        cnodes[1] = new SH_Node(c.PointAtEnd, null);

                        nodes.Add(cnodes[1]); //add bottom node to node list

                        SH_Line sh_col = new SH_Line(cnodes, _ss.elementCount++, "Column");
                        sh_col.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                        _ss.Elements["Line"].Add(sh_col);
                    }

                    else if (c.PointAtStart.Z < c.PointAtEnd.Z) //wants the node with the lowest Z value (bottom node)
                    {
                        SH_Node[] cnodes = new SH_Node[2];
                        cnodes[0] = new SH_Node(c.PointAtStart, null);
                        cnodes[1] = new SH_Node(c.PointAtEnd, null);

                        nodes.Add(cnodes[0]); //add bottom node to node list

                        SH_Line sh_col = new SH_Line(cnodes, _ss.elementCount++, "Column");
                        sh_col.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                        _ss.Elements["Line"].Add(sh_col);
                    }
                }
                //Add nodes to Simple Shape
                _ss.Nodes = new List<SH_Node>();
                _ss.Nodes.AddRange(nodes);
            }

            // ---------------------- SUBSTRUCTOR nr 3 -------------------- (bowed roof)
            if (NrSub == 3)
            {
                _ss.name += "_3";
                List<Curve> col = new List<Curve>(); //list to store columns before giving id
                List<SH_Node> nodes = new List<SH_Node>(); //empty list for nodes
                _ss.Elements["Line"] = new List<SH_Element>(); //empty list
                List<SH_Element> elementLst = _ss.Elements["Surface"];

                foreach (SH_Element element in elementLst)
                {
                    SH_Surface sh_srf = (SH_Surface)element;
                    string n = sh_srf.elementName;

                    if (n == "Top")
                    {
                        Surface s = sh_srf.elementSurface;

                        //find which side is the longest, store lines and nodes
                        if (s.IsoCurve(0, 0).GetLength() > s.IsoCurve(1, 1).GetLength() || s.IsoCurve(0, 0).GetLength() == s.IsoCurve(1, 1).GetLength())
                        {
                            Curve longBeam1 = s.IsoCurve(0, 0); //longitudinal beam = longest line of the surface
                            Curve longBeam2 = s.IsoCurve(0, 1);

                            SH_Node[] lnodes1 = new SH_Node[2];
                            lnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            lnodes1[1] = new SH_Node(longBeam1.PointAtEnd, null);
                            SH_Node[] lnodes2 = new SH_Node[2];
                            lnodes2[0] = new SH_Node(longBeam2.PointAtStart, null);
                            lnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);
                            SH_Node[] tnodes1 = new SH_Node[2];
                            tnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            tnodes1[1] = new SH_Node(longBeam2.PointAtStart, null);
                            SH_Node[] tnodes2 = new SH_Node[2];
                            tnodes2[0] = new SH_Node(longBeam1.PointAtEnd, null);
                            tnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);

                            //SH_Lines
                            SH_Line sh_longBeam1 = new SH_Line(lnodes1, _ss.elementCount++, "longBeam_Mitchell_3"); //longitudinal beam 1
                            sh_longBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_longBeam1);
                            SH_Line sh_longBeam2 = new SH_Line(lnodes2, _ss.elementCount++, "longBeam_Mitchell_3"); //longitudinal beam 2 
                            sh_longBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_longBeam2);
                            SH_Line sh_transBeam1 = new SH_Line(tnodes1, _ss.elementCount++, "transBeam_Mitchell_3"); //transversal beam 1
                            sh_transBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_transBeam1);
                            SH_Line sh_transBeam2 = new SH_Line(tnodes2, _ss.elementCount++, "transBeam_Mitchell_3"); //transversal beam 2
                            sh_transBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_transBeam2);

                            //store top nodes
                            nodes.Add(lnodes1[0]);
                            nodes.Add(lnodes1[1]);
                            nodes.Add(lnodes2[0]);
                            nodes.Add(lnodes2[1]);

                            // Create bowed roof, two arches
                            // Arch 1
                            Curve transBeam1 = s.IsoCurve(1, 0); //get correct line from surface, transversal
                            List<Point3d> bPts1 = new List<Point3d>();
                            bPts1.Add(transBeam1.PointAtStart);
                            Point3d midPt1 = transBeam1.PointAtNormalizedLength(0.5);
                            Point3d transMidPt1 = new Point3d(midPt1.X, midPt1.Y, midPt1.Z + H);
                            bPts1.Add(transMidPt1);
                            bPts1.Add(transBeam1.PointAtEnd);

                            Curve arch1 = Curve.CreateControlPointCurve(bPts1, 2); //create arch
                            Point3d[] bowedPts1;
                            arch1.DivideByCount(Convert.ToInt32(Count), true, out bowedPts1); // Divide arch into segments, store points

                            for (int j = 0; j < bowedPts1.Length - 1; j++)
                            {
                                Line bLine1 = new Line(bowedPts1[j], bowedPts1[j + 1]);
                                SH_Node[] bnodes1 = new SH_Node[2];
                                bnodes1[0] = new SH_Node(bLine1.From, null);
                                bnodes1[1] = new SH_Node(bLine1.To, null);
                                SH_Line sh_bowedRoof = new SH_Line(bnodes1, _ss.elementCount++, "bowedBeam_Mitchell_3"); // bowed roof at one side
                                sh_bowedRoof.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                                _ss.Elements["Line"].Add(sh_bowedRoof);
                            }

                            // Arch 2
                            Curve transBeam2 = s.IsoCurve(1, 1); //get correct line from surface, transversal
                            List<Point3d> bPts2 = new List<Point3d>();
                            bPts2.Add(transBeam2.PointAtStart);
                            Point3d midPt2 = transBeam2.PointAtNormalizedLength(0.5);
                            Point3d transMidPt2 = new Point3d(midPt2.X, midPt2.Y, midPt2.Z + H);
                            bPts2.Add(transMidPt2);
                            bPts2.Add(transBeam2.PointAtEnd);

                            Curve arch2 = Curve.CreateControlPointCurve(bPts2, 2); //create arch
                            Point3d[] bowedPts2;
                            arch2.DivideByCount(Convert.ToInt32(Count), true, out bowedPts2); // Divide arch into segments, store points

                            for (int k = 0; k < bowedPts2.Length - 1; k++)
                            {
                                Line bLine2 = new Line(bowedPts2[k], bowedPts2[k + 1]);
                                SH_Node[] bnodes2 = new SH_Node[2];
                                bnodes2[0] = new SH_Node(bLine2.From, null);
                                bnodes2[1] = new SH_Node(bLine2.To, null);
                                SH_Line sh_bowedRoof = new SH_Line(bnodes2, _ss.elementCount++, "bowedBeam_Mitchell_3"); // bowed roof at one side
                                sh_bowedRoof.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                                _ss.Elements["Line"].Add(sh_bowedRoof);
                            }

                            //store nodes from bowedPts1 and bowedPts2
                            for (int node1 = 0; node1 < bowedPts1.Length; node1++)
                            {
                                SH_Node[] bnodes = new SH_Node[1];
                                bnodes[0] = new SH_Node(bowedPts1[node1], null);
                                nodes.Add(bnodes[0]);
                            }
                            for (int node2 = 0; node2 < bowedPts2.Length; node2++)
                            {
                                SH_Node[] bnodes = new SH_Node[1];
                                bnodes[0] = new SH_Node(bowedPts2[node2], null);
                                nodes.Add(bnodes[0]);
                            }
                        }

                        else if (s.IsoCurve(0, 0).GetLength() < s.IsoCurve(1, 1).GetLength())
                        {
                            //get the longitudinal beams
                            Curve longBeam1 = s.IsoCurve(1, 1);
                            Curve longBeam2 = s.IsoCurve(1, 0);

                            SH_Node[] lnodes1 = new SH_Node[2];
                            lnodes1[0] = new SH_Node(longBeam1.PointAtStart, null);
                            lnodes1[1] = new SH_Node(longBeam1.PointAtEnd, null);
                            SH_Node[] lnodes2 = new SH_Node[2];
                            lnodes2[0] = new SH_Node(longBeam2.PointAtStart, null);
                            lnodes2[1] = new SH_Node(longBeam2.PointAtEnd, null);
                            SH_Node[] tnodes1 = new SH_Node[2];
                            tnodes1[0] = new SH_Node(longBeam1.PointAtEnd, null);
                            tnodes1[1] = new SH_Node(longBeam2.PointAtEnd, null);
                            SH_Node[] tnodes2 = new SH_Node[2];
                            tnodes2[0] = new SH_Node(longBeam1.PointAtStart, null);
                            tnodes2[1] = new SH_Node(longBeam2.PointAtStart, null);


                            //SH_Line
                            SH_Line sh_longBeam1 = new SH_Line(lnodes1, _ss.elementCount++, "longBeam_Mitchell_3"); //longitudinal beam 1
                            sh_longBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_longBeam1);
                            SH_Line sh_longBeam2 = new SH_Line(lnodes2, _ss.elementCount++, "longBeam_Mitchell_3"); //longitudinal beam 2 
                            sh_longBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_longBeam2);
                            SH_Line sh_transBeam1 = new SH_Line(tnodes1, _ss.elementCount++, "transBeam_Mitchell_3"); //transversal beam 1
                            sh_transBeam1.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_transBeam1);
                            SH_Line sh_transBeam2 = new SH_Line(tnodes2, _ss.elementCount++, "transBeam_Mitchell_3"); //transversal beam 2
                            sh_transBeam2.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                            _ss.Elements["Line"].Add(sh_transBeam2);

                            //store top nodes
                            nodes.Add(lnodes1[0]);
                            nodes.Add(lnodes1[1]);
                            nodes.Add(lnodes2[0]);
                            nodes.Add(lnodes2[1]);

                            // Create bowed roof, two arches
                            // Arch 1
                            Curve transBeam1 = s.IsoCurve(0, 1); //get correct line from surface, transversal
                            List<Point3d> bPts1 = new List<Point3d>();
                            bPts1.Add(transBeam1.PointAtStart);
                            Point3d midPt1 = transBeam1.PointAtNormalizedLength(0.5);
                            Point3d transMidPt1 = new Point3d(midPt1.X, midPt1.Y, midPt1.Z + H);
                            bPts1.Add(transMidPt1);
                            bPts1.Add(transBeam1.PointAtEnd);

                            Curve arch1 = Curve.CreateControlPointCurve(bPts1, 2); //create arch
                            Point3d[] bowedPts1;
                            arch1.DivideByCount(Convert.ToInt32(Count), true, out bowedPts1); // Divide arch into segments, store points

                            for (int j = 0; j < bowedPts1.Length - 1; j++)
                            {
                                Line bLine1 = new Line(bowedPts1[j], bowedPts1[j + 1]);
                                SH_Node[] bnodes1 = new SH_Node[2];
                                bnodes1[0] = new SH_Node(bLine1.From, null);
                                bnodes1[1] = new SH_Node(bLine1.To, null);
                                SH_Line sh_bowedRoof = new SH_Line(bnodes1, _ss.elementCount++, "bowedBeam_Mitchell_3"); // bowed roof at one side
                                sh_bowedRoof.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                                _ss.Elements["Line"].Add(sh_bowedRoof);
                            }

                            // Arch 2
                            Curve transBeam2 = s.IsoCurve(0, 0); //get correct line from surface, transversal
                            List<Point3d> bPts2 = new List<Point3d>();
                            bPts2.Add(transBeam2.PointAtStart);
                            Point3d midPt2 = transBeam2.PointAtNormalizedLength(0.5);
                            Point3d transMidPt2 = new Point3d(midPt2.X, midPt2.Y, midPt2.Z + H);
                            bPts2.Add(transMidPt2);
                            bPts2.Add(transBeam2.PointAtEnd);

                            Curve arch2 = Curve.CreateControlPointCurve(bPts2, 2); //create arch
                            Point3d[] bowedPts2;
                            arch2.DivideByCount(Convert.ToInt32(Count), true, out bowedPts2); // Divide arch into segments, store points

                            for (int k = 0; k < bowedPts2.Length - 1; k++)
                            {
                                Line bLine2 = new Line(bowedPts2[k], bowedPts2[k + 1]);
                                SH_Node[] bnodes2 = new SH_Node[2];
                                bnodes2[0] = new SH_Node(bLine2.From, null);
                                bnodes2[1] = new SH_Node(bLine2.To, null);
                                SH_Line sh_bowedRoof = new SH_Line(bnodes2, _ss.elementCount++, "bowedBeam_Mitchell_3"); // bowed roof at one side
                                sh_bowedRoof.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                                _ss.Elements["Line"].Add(sh_bowedRoof);
                            }

                            //store nodes from bowedPts1 and bowedPts2
                            for (int node1 = 0; node1 < bowedPts1.Length; node1++)
                            {
                                SH_Node[] bnodes = new SH_Node[1];
                                bnodes[0] = new SH_Node(bowedPts1[node1], null);
                                nodes.Add(bnodes[0]);
                            }
                            for (int node2 = 0; node2 < bowedPts2.Length; node2++)
                            {
                                SH_Node[] bnodes = new SH_Node[1];
                                bnodes[0] = new SH_Node(bowedPts2[node2], null);
                                nodes.Add(bnodes[0]);
                            }
                        }
                    }

                    // Find columns and bottom nodes
                    else if (n == "Shortest Wall")
                    {
                        Surface s = sh_srf.elementSurface;

                        //Find vertical curves of a surface = columns
                        Curve crv1 = s.IsoCurve(0, 0);
                        Vector3d vec1 = crv1.TangentAt(0.5);
                        Curve crv2 = s.IsoCurve(1, 1);
                        Vector3d vec2 = crv2.TangentAt(0.5);

                        //vertical curves = column
                        if (vec1.Z > 0)
                        {
                            Curve c1 = s.IsoCurve(0, 0);
                            Curve c2 = s.IsoCurve(0, 1);
                            col.Add(c1);
                            col.Add(c2);

                        }
                        else if (vec2.Z > 0)
                        {
                            Curve c1 = s.IsoCurve(1, 0);
                            Curve c2 = s.IsoCurve(1, 1);
                            col.Add(c1);
                            col.Add(c2);
                        }
                    }
                }
                //Columns and nodes
                for (int j = 0; j < col.Count; j++)
                {
                    Curve c = col[j];
                    if (c.PointAtStart.Z > c.PointAtEnd.Z) //wants the node with the lowest Z value (bottom node)
                    {
                        SH_Node[] cnodes = new SH_Node[2];
                        cnodes[0] = new SH_Node(c.PointAtStart, null);
                        cnodes[1] = new SH_Node(c.PointAtEnd, null);

                        nodes.Add(cnodes[1]); //add end node to node list

                        SH_Line sh_col = new SH_Line(cnodes, _ss.elementCount++, "Column");
                        sh_col.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                        _ss.Elements["Line"].Add(sh_col);
                    }

                    else if (c.PointAtStart.Z < c.PointAtEnd.Z) //wants the node with the lowest Z value (bottom node)
                    {
                        SH_Node[] cnodes = new SH_Node[2];
                        cnodes[0] = new SH_Node(c.PointAtStart, null);
                        cnodes[1] = new SH_Node(c.PointAtEnd, null);

                        nodes.Add(cnodes[0]); //add end node to node list

                        SH_Line sh_col = new SH_Line(cnodes, _ss.elementCount++, "Column");
                        sh_col.CrossSection = new SH_CrossSection_Beam(cSec, beamMat); // Add cross section and material to element 
                        _ss.Elements["Line"].Add(sh_col);
                    }
                }
                //Add all nodes to SH_Elements
                _ss.Nodes = new List<SH_Node>();
                _ss.Nodes.AddRange(nodes);
            }

            // Remove surfaces
            _ss.Elements["Surface"].RemoveAll(el => el.elementName == "Bottom");
            _ss.Elements["Surface"].RemoveAll(el => el.elementName == "Top");
            _ss.Elements["Surface"].RemoveAll(el => el.elementName == "Shortest Wall");
            _ss.Elements["Surface"].RemoveAll(el => el.elementName == "Longest Wall");

            // change the state
            _ss.SimpleShapeState = State.gamma;
            return "SubStructureRule successfully applied.";
        }


        public override State GetNextState()
        {
            return State.gamma;
        }

    }
}
