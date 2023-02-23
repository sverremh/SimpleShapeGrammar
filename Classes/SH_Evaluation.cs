using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using Grasshopper.Kernel;
using ShapeGrammar.Classes.Elements;

using Accord.Math;


namespace ShapeGrammar.Classes
{
    public static class SH_Evaluation
    {
        public static void ConstructMatrices(SG_Shape ss, out double[,] a, out double[] b)
        {
            // construct empty matrices
            double[,] a_1 = new double[ss.Nodes.Count, ss.Nodes.Count];
            double[] b_1 = new double[ss.Nodes.Count];

            // find index of supports:
            List<int> supInd = ss.Supports.Select(i => i.Node.ID).ToList(); // get the node index of the supports. 

            // assemble the matrices
            foreach (var node in ss.Nodes)
            {
                // if the node is a support, it should not be calculated. 
                if (supInd.Contains((int)node.ID))
                {
                    // if the node is a support we know that the moment is zero.
                    a_1[(int)node.ID, (int)node.ID] = 1;
                    continue;
                }

                // find adjacent elements
                List<SG_Elem1D> els = new List<SG_Elem1D>();
                foreach (var el in ss.Elems)
                {
                    if (el.Nodes.Contains(node)) // true if the node is connected to the element
                    {
                        els.Add((SG_Elem1D)el);
                    }
                }

                // get the ids of the adjacent nodes
                List<int> adj_IDs = new List<int>();
                foreach (var el in els)
                {
                    foreach (var n in el.Nodes)
                    {
                        if (node.ID != n.ID)
                        {
                            adj_IDs.Add((int)n.ID);
                        }
                    }
                }

                // calculate horizontal lengths. This should include y-coordinates as well. NOT IMPLEMENTED
                // calculate lenghts
                List<double> lengths = new List<double>();
                foreach (var nId in adj_IDs)
                {
                    double length = Math.Sqrt( Math.Pow(ss.Nodes[(int)node.ID].Pt.X - ss.Nodes[nId].Pt.X, 2) + Math.Pow(ss.Nodes[(int)node.ID].Pt.Y - ss.Nodes[nId].Pt.Y, 2));
                    lengths.Add(length);
                    a_1[(int)node.ID, nId] = length;
                }

                List<double> x_lengths = new List<double>();
                foreach (var nId in adj_IDs)
                {
                    double x_length = Math.Abs(ss.Nodes[(int)node.ID].Pt.X - ss.Nodes[nId].Pt.X);
                    //a_1[(int)node.ID, nId] = x_length;
                    x_lengths.Add(x_length);
                }
                // assign the final value from this node to the matrix
                a_1[(int)node.ID, (int)node.ID] = 2 * (lengths[0] + lengths[1]);

                b_1[(int)node.ID] = -(ss.LineLoads[0].Load.Z / 4) * (Math.Pow(lengths[0], 3) + Math.Pow(lengths[1], 3)); // should make a better way of retrieving the load from the SH_SimpleShape...

            }


            // remove columns and rows of the supports.
            // This is not really necessary if I modify the equations a bit to make it more robust. 
            //a_1 = a_1.RemoveColumn(supInd[0]); a_1 = a_1.RemoveColumn(supInd[1]-1);
            //a_1 = a_1.RemoveRow(supInd[0]); a_1 = a_1.RemoveRow(supInd[1]-1);
            //b_1 = b_1.RemoveAt(supInd[0]);
            //b_1 = b_1.RemoveAt(supInd[1]-1);

            // assign matrices to out references
            a = a_1;
            b = b_1;
        }
        public static double[] CalculateMoments(double[,] a, double[] b)
        {

            double[] moments = a.Solve(b);
            return moments;
        }
        public static double[] CalculateForces(SG_Shape _ss, double[] moments)
        {
            double load = _ss.LineLoads[0].Load.Z; // possible a better way to apply the loads if the element has several different ones. Future work...

            double[] forces = new double[moments.Length];
            for (int i = 0; i < _ss.Nodes.Count; i++)
            {
                // find adjacent elements
                List<SG_Element> els = new List<SG_Element>();
                foreach (var el in _ss.Elems)
                {
                    if (el.Nodes.Contains(_ss.Nodes[i])) // true if the node is connected to the element
                    {
                        els.Add(el);
                    }
                }
                // get the ids of the adjacent nodes
                List<int> adj_IDs = new List<int>();
                foreach (var el in els)
                {
                    foreach (var n in el.Nodes)
                    {
                        if (_ss.Nodes[i].ID != n.ID)
                        {
                            adj_IDs.Add((int)n.ID);
                        }
                    }
                }
                if (adj_IDs.Count == 1)
                {
                    double x_dist = Math.Abs(_ss.Nodes[i].Pt.X - _ss.Nodes[adj_IDs[0]].Pt.X);
                    double r_j = ((load / 2) * x_dist) + (moments[adj_IDs[0]] / x_dist) - (moments[(int)_ss.Nodes[i].ID] / x_dist);
                    forces[(int)_ss.Nodes[i].ID] = r_j;
                    continue;
                }
                if (adj_IDs.Count == 2)
                {
                    double x_dist1 = Math.Abs(_ss.Nodes[i].Pt.X - _ss.Nodes[adj_IDs[0]].Pt.X);
                    double x_dist2 = Math.Abs(_ss.Nodes[i].Pt.X - _ss.Nodes[adj_IDs[1]].Pt.X);

                    double r_j = (load / 2) * (x_dist1 + x_dist2) + moments[adj_IDs[0]] / x_dist1 + moments[adj_IDs[1]] / x_dist2 - moments[(int)_ss.Nodes[i].ID] / x_dist1 - moments[(int)_ss.Nodes[i].ID] / x_dist2;
                    forces[(int)_ss.Nodes[i].ID] = r_j;
                }

            }
            return forces;

        }

        public static double[] CalculateReactions(SG_Shape ss, double[] forces, double h_thrust)
        {

            // find index of supports 
            List<int> supInd = ss.Supports.Select(s => s.Node.ID).ToList();

            double r2 = 0.0;
            double sum = 0.0;
            foreach (var node in ss.Nodes)
            {
                int id = (int)node.ID;

                if (supInd.Contains(id)) continue; // No need to continue iteration if it is a support

                double x_dist = Math.Abs(ss.Nodes[id].Pt.X - ss.Nodes[supInd[0]].Pt.X);
                r2 += forces[id] * x_dist;
                sum += forces[id];
            }
            // add the horizontal thrust
            double h1 = ss.Nodes[0].Pt.Z;
            double h2 = ss.Nodes[1].Pt.Z;
            double delta_h = h1 - h2;
            double m_h = delta_h * h_thrust; // the moment from the horizontal thrust 
            r2 -= m_h;

            double totalLength = Math.Abs(ss.Nodes[supInd[1]].Pt.X - ss.Nodes[supInd[0]].Pt.X);
            r2 = r2 / totalLength;
            double r1 = sum - r2;

            return new double[] { r1, r2 };
        }
        public static Dictionary<string, List<Line> > DrawReciprocal(SG_Shape ss, double[] reactions, double[] forces, double h_thrust)
        {


            
            

            Dictionary<string, List<Line>> reciprocal_diagram = new Dictionary<string, List<Line>>();
            List<int> supInd = ss.Supports.Select(s => s.Node.ID).ToList();

            // test if reciprocal can be drawn
            if (ss.Elems.Count < 2)
            {
                throw new Exception("Not enough elements to draw funiculars. Minimum ");
            }


            // order the nodes correctly. 
            List<SG_Node> ordered_nodes = new List<SG_Node> { ss.Nodes[ supInd[0] ] };
            List<int> ordered_indices = new List<int> { 0 }; // list of the indices of the nodes after sorting            
            SortNodes(ss, ss.Nodes[supInd[0]], ref ordered_nodes, ref ordered_indices); 

            // draw lines for the external forces
            List<Line> external_forces = new List<Line>();
            Point3d line_start = new Point3d(h_thrust, -reactions[0], 0);
            //Point3d line_start = new Point3d(ePt); // copy the ePt of the first line

            int count = 0;
            foreach (var ind in ordered_indices)
            {
                if (count == 0 || count == ordered_indices.Count - 1)
                {
                    count++;
                    continue;
                }
                var end = new Point3d(line_start.X, line_start.Y + forces[ind], 0);
                var line = new Line(line_start, end);
                line_start = end;
                external_forces.Add(line);
                count++;
            }
            
            reciprocal_diagram.Add("external", external_forces);
            // Draw lines for element forces
            List<Line> internalForces = new List<Line>();
            Point3d o = new Point3d(0, 0, 0);
            foreach (var ext_line in external_forces)
            {
                Line l = new Line(o, ext_line.From);
                internalForces.Add(l);
            }
            internalForces.Add(new Line(o, external_forces[ external_forces.Count-1 ].To));

            reciprocal_diagram.Add("internal", internalForces);

            // Create the funicular elements and new nodes below: 
            List<double> heights = new List<double>();
            var start_node = ordered_nodes[0]; // start node
            var h0 = ordered_nodes[0].Pt.Z; // height at beginning
            for (int i = 0; i < ordered_indices.Count-1; i++)
            {
                var p1 = ordered_nodes[i].Pt; // first node
                var p2 = ordered_nodes[i + 1].Pt; // second node
                var f = reciprocal_diagram["internal"][i]; // force line from reciprocal
                //var h_i = (f.Direction.Y / f.Direction.X) * ( p1.DistanceTo(p2)); // the difference in height between this and the previous element
                var h_i = (f.Direction.Y / f.Direction.X) * (p2.X - p1.X);
                heights.Add(h_i);

                
                if(i < ordered_indices.Count - 2)
                {
                    // deck nodes
                    var s_node = ss.Nodes[ordered_indices[i]];
                    var e_node = ss.Nodes[ordered_indices[i+1]];


                    // add new node for the funicular
                    h0 += h_i;
                    var end_pos = new Point3d(e_node.Pt.X, e_node.Pt.Y, e_node.Pt.Z + h0); //This is wrong. Now all the lines starts in the same point.
                    SG_Node end_node = new SG_Node(end_pos, ss.nodeCount++);
                    ss.Nodes.Add(end_node);

                    // new element for funicular
                    SG_Node[] nodes = new SG_Node[] {start_node , end_node};
                    string name = "funicular";
                    int id = ss.elementCount++;
                    SG_Elem1D funicular = new SG_Elem1D(nodes, id, name);
                    ss.Elems.Add(funicular);

                    // new element for verticals
                    string name_vert = "verticals";
                    int id_v = ss.elementCount++;
                    SG_Node[] nodes_v = new SG_Node[] { e_node, end_node };
                    SG_Elem1D ve = new SG_Elem1D(nodes_v, id_v, name_vert);
                    start_node = end_node;
                    ss.Elems.Add(ve);
                }
                if(i == ordered_indices.Count - 2)
                {
                    
                    // only adding the element
                    SG_Node[] nodes = new SG_Node[] { start_node, ss.Nodes[1] };
                    string name = "funicular";
                    int id = ss.elementCount++;
                    SG_Elem1D funicular = new SG_Elem1D(nodes, id, name);
                    ss.Elems.Add(funicular);
                }
            }

            return reciprocal_diagram;
        }

        public static List<Line> DrawFunicular(SG_Shape ss, Dictionary<string, List<Line>> reciprocal)
        {
            List<Line> funiculars = new List<Line>();

            Point3d startPt = ss.Nodes[0].Pt;
            
            // To calculate this I need: 
                // - The length of the bar underneath (from sorted nodes maybe?)
                // - The vector representing the direction of each line from the reciprocal diagram. 
                // - The formular for finding the height of the new node: h_n = l_deck * ( y / x)

            return funiculars; 
        }
        public static void SortNodes(SG_Shape ss, SG_Node node, ref List<SG_Node> nodes, ref List<int> sort_ind)
        {
            
            List<SG_Element> els = ss.Elems.Where(el => el.Nodes.Contains(node)).ToList();       // find the elements adjacent to the node
            SG_Node new_node = new SG_Node();
            if(els.Count == 1)
            {
                var el = els[0];
                int new_ind = el.Nodes.First(n => n.ID != node.ID);
                new_node = el.Nodes[new_ind];
                nodes.Add(new_node);
                sort_ind.Add(ss.Nodes.IndexOf(new_node));
                       
            }
            else
            {
                // find the element where only one node is already in the sorted list.
                foreach (var el in els)
                {
                    if (nodes.Contains(el.Nodes[0]) && nodes.Contains(el.Nodes[1])) continue;       // if both nodes are already sorted, we are not interested in this element

                    int new_ind = el.Nodes.First(n => n.ID != node.ID);
                    new_node = el.Nodes[new_ind];
                    nodes.Add(new_node);
                    sort_ind.Add(ss.Nodes.IndexOf(new_node));

                }
            }
            if (nodes.Count < ss.Nodes.Count)
            {
                SortNodes(ss, new_node, ref nodes, ref sort_ind);
            }

        }
        
    }
}
