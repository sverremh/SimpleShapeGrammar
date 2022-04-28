﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using SimpleShapeGrammar.Classes;
using SimpleShapeGrammar.Classes.Elements;

namespace SimpleShapeGrammar.Classes.Rules
{
    /// <summary>
    /// Rule splitting the line at the input parameter
    /// </summary>
    [Serializable]
    public class SH_Rule02 : SH_Rule
    {
        // --- properties ---
        //public State RuleState = State.beta;

        public int LineIndex { get; set; }
        public double Param { get; set; }
        private double[] bounds = { 0.2, 0.8 };


        // --- constructors ---
        public SH_Rule02()
        {
            RuleState = State.beta;
            Name = "SH_Rule02";
        }

        public SH_Rule02(int _line_index, double _param)
        {
            Name = "SH_Rule02";
            LineIndex = _line_index;
            Param = _param;
            RuleState = State.beta;
        }

        // --- methods ---
        public override SH_Rule CopyRule(SH_Rule rule)
        {
            throw new NotImplementedException();
        }

        public override string RuleOperation(SH_SimpleShape _ss)
        {
            // check if the state maches the simple shape state
            if (_ss.SimpleShapeState != RuleState)
            {
                return "The current state is not compatible with Rule02.";
            }

            // choose the line to split
            //SH_Element line = _ss.Lines.Where(l => l.ID == LineID).First(); DELETE IF OK
            SH_Line line = new SH_Line();
            // to do: evaluate if this is the best method for avoiding an index out of range error. 
            try
            {
                line = (SH_Line)_ss.Elements["Line"][LineIndex];
            }
            catch (Exception ex)
            {
                if (ex is ArgumentOutOfRangeException || ex is IndexOutOfRangeException)
                {
                    line = (SH_Line)_ss.Elements["Line"][_ss.elementCount - 1]; // if out of range, take the last item
                }

            }

            double line_length = line.Nodes[0].Position.DistanceTo(line.Nodes[1].Position);
            double segment1 = line_length * Param;
            double segment2 = line_length * (1 - Param);
            // test if the line original length or the splitted segments are smaller than 1 meter. If true the rule cannot be applied. 
            if ((line_length < 1.0) || (segment1 < 1.0) || (segment2 < 1.0))
            {
                return "The line segment are too short for the rule to be applied on this element";
            }

            int elInd = _ss.Elements["Line"].IndexOf(line);
            // add the intermediate node
            SH_Node newNode = AddNode(line, Param, _ss.nodeCount);
            _ss.Nodes.Add(newNode);
            _ss.nodeCount++;

            // create 2x lines 
            List<SH_Node> nodes = new List<SH_Node>();

            //SH_Element newLine0 = new SH_Element(new SH_Node[] { line.Nodes[0], newNode }, _ss.elementCount, line.elementName); // add the element name here too. DELETE if line below is working!
            SH_Line newLine0 = new SH_Line(new SH_Node[] { line.Nodes[0], newNode }, line.ID, line.elementName); // add the element name here too.
            //_ss.elementCount++;  DELETE if above method is working
            SH_Line newLine1 = new SH_Line(new SH_Node[] { newNode, line.Nodes[1] }, _ss.elementCount, line.elementName);
            _ss.elementCount++;



            // remove the line which has been split
            _ss.Elements["Line"].RemoveAt(elInd);
            _ss.Elements["Line"].Insert(elInd, newLine1);

            // insert the new lines in its position
            _ss.Elements["Line"].Insert(elInd, newLine0);

            // no change in the state (remains in beta state)
            // _ss.SimpleShapeState = State.beta; 

            return "Rule02 successfully applied!";

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_line"> The line to operate on</param>
        /// <param name="_t"> Splitting parameter</param>
        /// <param name="_id">ID of the line</param>
        /// <returns></returns>
        private SH_Node AddNode(SH_Element _line, double _t, int _id)
        {
            double mx = (1 - _t) * _line.Nodes[0].Position.X + _t * _line.Nodes[1].Position.X;
            double my = (1 - _t) * _line.Nodes[0].Position.Y + _t * _line.Nodes[1].Position.Y;
            double mz = (1 - _t) * _line.Nodes[0].Position.Z + _t * _line.Nodes[1].Position.Z;
            Point3d newPoint = new Point3d(mx, my, mz);
            SH_Node newNode = new SH_Node(newPoint, _id);

            return newNode;
        }
        /// <summary>
        /// Set the lower bound of the split parameter
        /// </summary>
        /// <param name="lower"></param>
        public void SetLowerBound(double lower)
        {
            bounds[0] = lower;
        }
        /// <summary>
        /// Gets the lower bound of the split parameter
        /// </summary>
        /// <returns> lower bound </returns>
        public double GetLowerBound()
        {
            return bounds[0];
        }
        /// <summary>
        /// Set the upper bound of the split parameter
        /// </summary>
        /// <param name="upper"></param>
        public void SetUpperBound(double upper)
        {
            bounds[1] = upper;
        }
        /// <summary>
        /// Gets the upper bound of the split parameter
        /// </summary>
        /// <returns>Upper bound</returns>
        public double GetUpperBound()
        {
            return bounds[1];
        }

        public override void NewRuleParameters(Random random, SH_SimpleShape ss)
        {
            // the parameter to use for the rule
            int numLines = ss.elementCount;
            Param = SH_UtilityClass.RandomExtensions.NextDouble(random, bounds[0], bounds[1]);
            LineIndex = random.Next(0, numLines - 1);

        }

        public override State GetNextState()
        {
            return State.beta;
        }
    }
}
