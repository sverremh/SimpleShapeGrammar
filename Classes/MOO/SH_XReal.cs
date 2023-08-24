using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JMetalCSharp.Utils.Wrapper;
using JMetalCSharp.Core;

using ShapeGrammar.Classes.Rules;

namespace ShapeGrammar.Classes
{
    public class SH_XReal : XReal
    {
        private SH_Solution SH_solution;
        private SolutionType type;

        public SH_XReal()
        {
            // empty
        }
        public SH_XReal(SH_Solution solution)
        {
            this.SH_solution = solution;
            type = solution.Type;
        }
        /// <summary>
        /// Returns the list of rules used by the genome
        /// </summary>
        /// <returns></returns>
        public List<SG_Rule> GetRuleList()
        {
            List<SG_Rule> results;
            Type type = this.type.GetType();
            if (type == typeof(SH_SolutionType))
            {
                SH_Variable variable = (SH_Variable)SH_solution.SH_Variable[0];
                results = variable.RuleList;
            }
            else
            {
                results = null;
                // to do: log error message
            }
            return results;
        }

        public SG_Shape GetSimpleShape()
        {
            SG_Shape ss;
            Type type = this.type.GetType();
            if (type == typeof(SH_SolutionType))
            {
                SH_Variable variable = (SH_Variable)SH_solution.SH_Variable[0];
                ss = variable.SimpleShape;
            }

            else
            {
                ss = null;
            }

            return ss;
        }


        /// <summary>
        /// Sets the list of rules used by the genome
        /// </summary>
        /// <param name="ruleList"></param>
        public void SetRuleList(List<SG_Rule> ruleList)
        {
            Type type = this.type.GetType();
            // List<SH_Rule> results;

            if (type == typeof(SH_SolutionType))
            {
                ((SH_Variable)SH_solution.SH_Variable[0]).RuleList = ruleList; 
            }
            else
            {
                // to do: log error message
            }
        }
        /// <summary>
        /// Returns the number of Rules used in the genome
        /// </summary>
        /// <returns></returns>
        public int GetLengthOfRules()
        {
            return ((SH_Variable)SH_solution.SH_Variable[0]).RuleList.Count;
        }
    }
}
