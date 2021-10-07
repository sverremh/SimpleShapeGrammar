using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JMetalCSharp.Utils.Wrapper;
using JMetalCSharp.Core;

namespace SimpleShapeGrammar.Classes
{
    public class SH_XReal : XReal
    {
        private Solution solution;
        private SolutionType type;

        public SH_XReal()
        {
            // empty
        }
        public SH_XReal(Solution solution)
        {
            this.solution = solution;
            type = solution.Type;
        }
        /// <summary>
        /// Returns the list of rules used by the genome
        /// </summary>
        /// <returns></returns>
        public List<SH_Rule> GetRuleList()
        {
            List<SH_Rule> results;
            Type type = this.type.GetType();
            if (type == typeof(SH_SolutionType))
            {
                SH_Variable variable = (SH_Variable)solution.Variable[0];
                results = variable.RuleList;
            }
            else
            {
                results = null;
                // to do: log error message
            }
            return results;
        }

        
        /// <summary>
        /// Sets the list of rules used by the genome
        /// </summary>
        /// <param name="ruleList"></param>
        public void SetRuleList(List<SH_Rule> ruleList)
        {
            Type type = this.type.GetType();
            List<SH_Rule> results;

            if (type == typeof(SH_SolutionType))
            {
                ((SH_Variable)solution.Variable[0]).RuleList = ruleList; 
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
            return ((SH_Variable)solution.Variable[0]).RuleList.Count;
        }
    }
}
