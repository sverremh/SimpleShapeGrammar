using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleShapeGrammar.Classes;

namespace SimpleShapeGrammar.Classes
{
    /// <summary>
    /// Utility class for methods related to the multi-objective optimisation
    /// </summary>
    [Serializable]
    public static class MOO_Utility
    {
        public static List<SH_Rule> NewGenome(List<object> rules, List<double> weights, Random random)
        {
            List<SH_Rule> genome = new List<SH_Rule>(); // initialise the genome
            State state = State.alpha; // set the initial state of the rules
            int count = 0;
            // add rules to the genome
            while (count < 20 && state != State.end )
            {
                object listElement;
                SH_UtilityClass.TakeRandomItem(rules, weights, random, out listElement);
                var ruleString = listElement as string;
                if (ruleString != null)
                {
                    var ruleObject = NewRule(ruleString) as SH_Rule;

                    if (ruleObject != null)
                    {
                        ruleObject.NewRuleParameters(random);
                        genome.Add(ruleObject);
                    }
                    else
                    {
                        throw new Exception("There was an error when taking a rule from the list");
                    }

                    state = ruleObject.RuleState;

                }
                else
                {
                    throw new Exception("There was an error when taking the element from the list");
                }
                count++;
            }

            /*
            // apply new rule until the final rule is selected.
            while (state != State.end)
            {
                var randomRule = 
            }
            */



            return genome;
        }
        public static object TestActivator()
        {
            //var objectToInstantiate = typeof(SH_Rule).AssemblyQualifiedName;
            const string objectToInstantiate = "SimpleShapeGrammar.Classes.SH_Rule01, SimpleShapeGrammar";
            var objectType = Type.GetType(objectToInstantiate);

            var instantiateObject = Activator.CreateInstance(objectType) as ISH_Rule; // use the interface to make the "NewRule()" possible

            // test if something can be done with the rule
            Random rnd = new Random();
            instantiateObject.NewRuleParameters(rnd); // Set the translation vectors for this

            return instantiateObject;
        }
        public static object NewRule(string ruleStr)
        {
            // Make this more general to 
            string ruleToInitiate = "SimpleShapeGrammar.Classes." + ruleStr + ", SimpleShapeGrammar";
            Type type = Type.GetType(ruleToInitiate);

            if (type != null)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }        
    }
}
