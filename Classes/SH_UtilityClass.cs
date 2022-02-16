using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Grasshopper.Kernel;
using Rhino.Geometry;
using SimpleShapeGrammar.Classes.Elements;

using Karamba.Models;
using Karamba.Utilities;
using Karamba.Elements;
using Karamba.Geometry;
using Karamba.GHopper.Geometry;
using Karamba.CrossSections;
using Karamba.Supports;
using Karamba.Loads;

namespace SimpleShapeGrammar.Classes
{

    // --- variables ---

    // --- input ---

    // --- solve ---

    // --- output ---


    // --- properties ---

    // --- constructors ---

    // --- methods ---

    public static class SH_UtilityClass
    {
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
        public static SH_SimpleShape ApplyRulesToSimpleShape(List<SH_Rule> rules, SH_SimpleShape ss)
        {
            SH_SimpleShape ssCopy = ss.DeepCopy() ; 
            foreach (SH_Rule rule in rules)
            {
                try
                {
                    string message = rule.RuleOperation(ssCopy);
                    //comp.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, message);
                }
                catch (Exception ex)
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
        public static Model Karamba3DModelFromSimpleShape(SH_SimpleShape ss)
        {
            var nodes = new List<Point3>();
            var logger = new MessageLogger();
            var k3d = new KarambaCommon.Toolkit();
            SH_SimpleShape simpleShape = ss.DeepCopy();
            // create karamba Line3 elements
            List<string> element_names;
            List<Line3> k_lines = SH_ElementsToKarambaLines(simpleShape.Elements["Line"], k3d, out element_names);

            // create Karamba Builder Beams from Line3 list. 
            List<BuilderBeam> elems = k3d.Part.LineToBeam(k_lines, element_names,
                new List<CroSec>(), logger, out nodes);
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
                Point3 loc = new Point3(sup.Position.X, sup.Position.Y, sup.Position.Z);

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
            double mass;
            Point3 cog; // centre of gravity for the model
            bool flag;
            string info;
            Model model = k3d.Model.AssembleModel(elems, supports, loads,
                out info, out mass, out cog, out info, out flag);

            return model;


        }

        public static List<double> AnalyseKarambaModel(List<string> objectives, Model model)
        {
            List<double> results = new List<double>();

            var k3d = new KarambaCommon.Toolkit();
            // calculate Th.I response
            List<double> max_disp;
            List<double> out_g;
            List<double> out_comp;
            string message;

            try
            {
                Model analysedModel = k3d.Algorithms.AnalyzeThI(model, out max_disp, out out_g, out out_comp, out message);
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
            catch (Exception ex)
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
        private static List<Line3> SH_ElementsToKarambaLines(List<SH_Element> elements, KarambaCommon.Toolkit k3d, out List<string> el_names)
        {
            // initiate list

            List<Line3> k_lines = new List<Line3>();
            List<string> k_names = new List<string>();
            // create karamabe BuilderBeam elements using Factory method
            foreach (SH_Element el in elements)
            {
                // get node points
                Point3d sPt = el.Nodes[0].Position;
                Point3d ePt = el.Nodes[1].Position;
                // convert to karamba's Point3
                Point3 k_sPt = new Point3(sPt.X, sPt.Y, sPt.Z);
                Point3 k_ePt = new Point3(ePt.X, ePt.Y, ePt.Z);

                // create Line3
                Line3 k_line = new Line3(k_sPt, k_ePt);
                k_lines.Add(k_line);

                // add name
                k_names.Add(el.elementName);


            }
            el_names = k_names;
            return k_lines;
        }

        public static void TakeRandomItem(List<object> fromList, List<double> weights, Random random, out object item)
        {
            // find the sum of weights
            double sum_of_weights = weights.Sum();
            object el = new object();
            int ind = 0;
            // initiate random
            //var random = new Random();
            double rnd = RandomExtensions.NextDouble(random, 0, sum_of_weights);
            //Console.WriteLine("Random number selected: {0}", rnd);
            for (int i = 0; i < weights.Count; i++)
            {
                if (rnd < weights[i])
                {
                    el = fromList[i];
                    ind = i;
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
