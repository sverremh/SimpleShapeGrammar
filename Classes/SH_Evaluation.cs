using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using Accord.Math;


namespace SimpleShapeGrammar.Classes
{
    public static class SH_Evaluation
    {
        public static void ConstructMatrices(SH_SimpleShape ss, out double[,] a, out double[] b)
        {
            // construct empty matrices
            double[,] a_1 = new double[ss.Nodes.Count, ss.Nodes.Count];
            double[] b_1 = new double[ss.Nodes.Count];

            // find index of supports:
            List<int> supInd = ss.Supports.Select(i => i.nodeInd).ToList(); // get the node index of the supports. 

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
                List<SH_Element> els = new List<SH_Element>();
                foreach (var el in ss.Elements)
                {
                    if (el.Nodes.Contains(node)) // true if the node is connected to the element
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
                        if (node.ID != n.ID)
                        {
                            adj_IDs.Add((int)n.ID);
                        }
                    }
                }

                // calculate horizontal lengths
                List<double> x_lengths = new List<double>();
                foreach (var nId in adj_IDs)
                {
                    double x_length = Math.Abs(ss.Nodes[(int)node.ID].Position.X - ss.Nodes[nId].Position.X);
                    a_1[(int)node.ID, nId] = x_length;
                    x_lengths.Add(x_length);
                }
                // assign the final value from this node to the matrix
                a_1[(int)node.ID, (int)node.ID] = 2 * (x_lengths[0] + x_lengths[1]);

                b_1[(int)node.ID] = -(ss.LineLoads[0].Load.Z / 4) * (Math.Pow(x_lengths[0], 3) + Math.Pow(x_lengths[1], 3)); // should make a better way of retrieving the load from the SH_SimpleShape...

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
        public static double[] CalculateForces(SH_SimpleShape _ss, double[] moments)
        {
            double load = _ss.LineLoads[0].Load.Z; // possible a better way to apply the loads if the element has several different ones. Future work...

            double[] forces = new double[moments.Length];
            for (int i = 0; i < _ss.Nodes.Count; i++)
            {
                // find adjacent elements
                List<SH_Element> els = new List<SH_Element>();
                foreach (var el in _ss.Elements)
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
                    double x_dist = Math.Abs(_ss.Nodes[i].Position.X - _ss.Nodes[adj_IDs[0]].Position.X);
                    double r_j = ((load / 2) * x_dist) + (moments[adj_IDs[0]] / x_dist) - (moments[(int)_ss.Nodes[i].ID] / x_dist);
                    forces[(int)_ss.Nodes[i].ID] = r_j;
                    continue;
                }
                if (adj_IDs.Count == 2)
                {
                    double x_dist1 = Math.Abs(_ss.Nodes[i].Position.X - _ss.Nodes[adj_IDs[0]].Position.X);
                    double x_dist2 = Math.Abs(_ss.Nodes[i].Position.X - _ss.Nodes[adj_IDs[1]].Position.X);

                    double r_j = (load / 2) * (x_dist1 + x_dist2) + moments[adj_IDs[0]] / x_dist1 + moments[adj_IDs[1]] / x_dist2 - moments[(int)_ss.Nodes[i].ID] / x_dist1 - moments[(int)_ss.Nodes[i].ID] / x_dist2;
                    forces[(int)_ss.Nodes[i].ID] = r_j;
                }

            }
            return forces;
        }

        public static double[] CalculateReactions(SH_SimpleShape ss, double[] forces)
        {

            // find index of supports 
            List<int> supInd = ss.Supports.Select(n => n.nodeInd).ToList();

            double r1 = 0.0;
            double sum = 0.0;
            foreach (var node in ss.Nodes)
            {
                int id = (int)node.ID;

                if (supInd.Contains(id)) continue; // No need to continue iteration if it is a support

                double x_dist = Math.Abs(ss.Nodes[id].Position.X - ss.Nodes[supInd[0]].Position.X);
                r1 += forces[id] * x_dist;
                sum += forces[id];
            }
            double totalLength = Math.Abs(ss.Nodes[supInd[1]].Position.X - ss.Nodes[supInd[0]].Position.X);
            r1 = r1 / totalLength;
            double r2 = sum - r1;

            return new double[] { r1, r2 };
        }
        public static Dictionary<string, List<Line> > DrawReciprocal(SH_SimpleShape ss, double[] reactions, double[] forces)
        {
            double horizontal_thrust = 50;
            //double horizontal_thrust = forces.Max() * 0.8; // this should be an optional input in later implementations as a part of Rule03
            Dictionary<string, List<Line>> reciprocal_diagram = new Dictionary<string, List<Line>>();
            List<int> supInd = ss.Supports.Select(s => s.nodeInd).ToList();


            // order the nodes correctly. 
            List<SH_Node> ordered_nodes = new List<SH_Node> { ss.Nodes[ supInd[0] ] };
            List<int> ordered_indices = new List<int> { 0 }; // list of the indices of the nodes after sorting            
            SortNodes(ss, ss.Nodes[supInd[0]], ref ordered_nodes, ref ordered_indices); 

            // draw lines for the external forces
            List<Line> external_forces = new List<Line>();
            Point3d line_start = new Point3d(horizontal_thrust, -reactions[0], 0);
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
            return reciprocal_diagram;
        }

        public static List<Line> DrawFunicular(SH_SimpleShape ss, Dictionary<string, List<Line>> reciprocal)
        {
            List<Line> funiculars = new List<Line>();

            Point3d startPt = ss.Nodes[0].Position;
            
            // To calculate this I need: 
                // - The length of the bar underneath (from sorted nodes maybe?)
                // - The vector representing the direction of each line from the reciprocal diagram. 
                // - The formular for finding the height of the new node: h_n = l_deck * ( y / x)

            return funiculars; 
        }
        public static void SortNodes(SH_SimpleShape ss, SH_Node node, ref List<SH_Node> nodes, ref List<int> sort_ind)
        {
            
            List<SH_Element> els = ss.Elements.Where(el => el.Nodes.Contains(node)).ToList();       // find the elements adjacent to the node
            SH_Node new_node = new SH_Node();
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
