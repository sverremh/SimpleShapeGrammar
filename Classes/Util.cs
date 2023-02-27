using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Grasshopper.Kernel;
using Rhino.Geometry;

using ShapeGrammar.Classes.Elements;
using ShapeGrammar.Classes.Rules;

using Karamba.Models;
using Karamba.Utilities;
using Karamba.Elements;
using Karamba.Geometry;
using Karamba.GHopper.Geometry;
using Karamba.CrossSections;
using Karamba.Supports;
using Karamba.Loads;

namespace ShapeGrammar.Classes
{
    

    // --- variables ---

    // --- input ---

    // --- solve ---

    // --- output ---


    // --- properties ---

    // --- constructors ---

    // --- methods ---

    public static class Util
    {
        public static double PRES = 0.001;

        public static double MIN_SEG_LEN = 1.0;

        public static int RULE_END_MARKER = -999;

        public static int RULE01_MARKER = -1;
        public static int RULE02_MARKER = -2;
        public static int RULE04_MARKER = -4;

        public static string CAT = "SimpleGrammar";
        public static string GR_RLS = "04. Rules";
        public static string GR_INT = "07. Interpreter";
        public static string GR_UTIL = "99. Utilities";

        public static T DeepCopy<T>(T target)
        {
            T result;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream m = new MemoryStream();

            try
            {
                bf.Serialize(m, target);
                m.Position = 0;
                result = (T)bf.Deserialize(m);
            }
            finally
            {
                m.Close();
            }



            return result;
        }
        /// <summary>
        /// Works as a grammar interpreter by applying the list of rules to the shape
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="ss"></param>
        public static SG_Shape ApplyRulesToSimpleShape(List<SG_Rule> rules, SG_Shape ss)
        {
            SG_Shape ssCopy = Util.DeepCopy(ss) ; 
            foreach (SG_Rule rule in rules)
            {
                try
                {
                    string message = rule.RuleOperation(ref ssCopy);
                    //comp.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, message);
                }
                catch // (Exception ex)
                {
                    //comp.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, ex.Message);;
                }

            }
            return ssCopy;
        }
        /// <summary>
        /// Builds a Karamba3d model from the components simple shape
        /// </summary>
        /// <param name="ss">Simple shape model to create Karamba model from.</param>
        /// <returns></returns>
        public static Model Karamba3DModelFromSimpleShape(SG_Shape ss)
        {
            // var nodes = new List<Point3>();
            var logger = new MessageLogger();
            var k3d = new KarambaCommon.Toolkit();
            SG_Shape simpleShape = ss.DeepCopy();
            // create karamba Line3 elements
            List<Line3> k_lines = SH_ElementsToKarambaLines(simpleShape.Elems, k3d, out List<string> element_names);

            // create Karamba Builder Beams from Line3 list. 
            List<BuilderBeam> elems = k3d.Part.LineToBeam(k_lines, element_names,
                new List<CroSec>(), logger, out _);
            /// <summary>
            /// not implemented: The cross section which should also come from the SH_SimpleElement.
            /// </summary>
            /// 

            // -- supports --
            // not implemented yet. Read through all the nodes. Create a support instance if the condition theres is not 0
            var supports = new List<Support>();
            foreach (var sup in simpleShape.Supports)
            {
                // karamba point
                Point3 loc = new Point3(sup.Node.Pt.X, sup.Node.Pt.Y, sup.Node.Pt.Z);

                // conditions
                List<bool> conditions = CreateBooleanConditions(sup.SupportCondition);

                // not implemented: Optional Plane

                // create support
                Support gh_sup = k3d.Support.Support(loc, conditions);
                supports.Add(gh_sup);
            }

            // -- loads --

            ///<summary>
            ///Note: Neither gravity load nor line load has a k3d (factory) initatior. 
            /// </summary>

            var loads = new List<Load>();
            // gravity load
            var gLoad = new GravityLoad(new Vector3(0, 0, -1), "0");
            loads.Add(gLoad);

            // line loads
            var lineLoads = new List<Load>();
            foreach (var l in simpleShape.LineLoads)
            {
                //var ids = l.ElementIds;
                var ids = l.ElementId;
                var k_vec = new Vector3(l.Load.X, l.Load.Y, l.Load.Z);
                var orient = LoadOrientation.global;
                int lc = l.LoadCase;

                var k_lineLoad = k3d.Load.ConstantForceLoad(k_vec.Unitized, k_vec.Length, 0.0, 1.0, orient, lc.ToString(), ids); // new Karamba version
                //var k_lineLoad = new UniformlyDistLoad(ids, k_vec, orient, lc);

                lineLoads.Add(k_lineLoad);
            }

            loads.AddRange(lineLoads);


            // point loads : not implemented yet

            // -- assembly --
            // centre of gravity for the model
            Model model = k3d.Model.AssembleModel(elems, supports, loads,
                out _, out _, out _, out _, out _);

            return model;


        }

        public static List<double> AnalyseKarambaModel(List<string> objectives, Model model)
        {
            List<double> results = new List<double>();

            var k3d = new KarambaCommon.Toolkit();
            // calculate Th.I response

            try
            {
                Model analysedModel = k3d.Algorithms.AnalyzeThI(model, out List<double> max_disp, out List<double> out_g, out List<double> out_comp, out string message);
                // iterate through each objective function
                foreach (string objective in objectives)
                {
                    if (objective == "max displacement")
                    {
                        results.Add(max_disp.Max()); // the maximum displacement for all present loadcases

                    }
                    else if (objective == "total mass")
                    {
                        results.Add(model.mass());
                    }
                    // to do: Add more possibilities for optimisation objectives. Eigenfrequencies, buckling factor, etc.


                }
            }
            catch // (Exception ex)
            {
                // to do: log this error
                // if there is an exception, there is an error in the model. Add high objective values to avoid these solution. 
                for (int i = 0; i < objectives.Count; i++)
                {
                    results.Add(double.PositiveInfinity);
                }
            }



            return results;
        }

        /// <summary>
        /// Private method for creating the boolean support conditions from the simple shape conditions
        /// </summary>
        /// <param name="condInt"></param>
        /// <returns></returns>
        private static List<bool> CreateBooleanConditions(int condInt)
        {
            List<bool> conditions = new List<bool>() { false, false, false, false, false, false };
            int val = condInt;
            for (int i = 5; i >= 0; i--)
            {
                int rest = (int)Math.Pow(2, i);
                if (val - rest >= 0)
                {
                    conditions[i] = true;
                    val -= rest;
                }
            }
            return conditions;
        }
        /// <summary>
        /// Private method to create SH_Elements into Karamba3D lines
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="k3d"></param>
        /// <param name="el_names"></param>
        /// <returns></returns>
        private static List<Line3> SH_ElementsToKarambaLines(List<SG_Element> elements, KarambaCommon.Toolkit k3d, out List<string> el_names)
        {
            // initiate list

            List<Line3> k_lines = new List<Line3>();
            List<string> k_names = new List<string>();
            // create karamabe BuilderBeam elements using Factory method
            foreach (SG_Element el in elements)
            {
                // get node points
                Point3d sPt = el.Nodes[0].Pt;
                Point3d ePt = el.Nodes[1].Pt;
                // convert to karamba's Point3
                Point3 k_sPt = new Point3(sPt.X, sPt.Y, sPt.Z);
                Point3 k_ePt = new Point3(ePt.X, ePt.Y, ePt.Z);

                // create Line3
                Line3 k_line = new Line3(k_sPt, k_ePt);
                k_lines.Add(k_line);

                // add name
                k_names.Add(el.Name);


            }
            el_names = k_names;
            return k_lines;
        }

        public static void TakeRandomItem(List<object> fromList, List<double> weights, Random random, out object item)
        {
            // find the sum of weights
            double sum_of_weights = weights.Sum();
            object el = new object();
            // initiate random
            //var random = new Random();
            double rnd = RandomExtensions.NextDouble(random, 0, sum_of_weights);
            //Console.WriteLine("Random number selected: {0}", rnd);
            for (int i = 0; i < weights.Count; i++)
            {
                if (rnd < weights[i])
                {
                    el = fromList[i];
                    break;
                }
                rnd -= weights[i];
            }
            item = el;
        }

        public static class RandomExtensions
        {
            public static double NextDouble(Random random, double min, double max)
            {
                return random.NextDouble() * (max - min) + min;
            }
        }
    }
}
