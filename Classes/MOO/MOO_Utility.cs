﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SimpleShapeGrammar.Classes;
using SimpleShapeGrammar.Classes.Rules;

namespace SimpleShapeGrammar.Classes
{
    /// <summary>
    /// Utility class for methods related to the multi-objective optimisation
    /// </summary>
    [Serializable]
    public static class MOO_Utility
    {
        public static List<SH_Rule> NewGenome(SH_Variable variable, List<object> rules, List<double> weights,
            Random random, SH_SimpleShape _ss)
        {
            var simpleShape = SH_UtilityClass.DeepCopy(_ss); // copy the default simple shape. 

            List<SH_Rule> genome = new List<SH_Rule>(); // instantiate the list for the genome.
            int numLines = 1; // count the maximum number of line elements in generated by this genome.

            State state = State.alpha; // set the initial state of the rules
            int count = 0;
            // add rules to the genome
            while (state != State.end)
            {
                SH_UtilityClass.TakeRandomItem(rules, weights, random, out var listElement);
                var ruleObject = listElement as SH_Rule;
                if (ruleObject != null)
                {
                    //var ruleObject = NewRule(ruleString) as SH_Rule; // insted of this, try to take random rule from list of available rules. 

                    //SH_UtilityClass.TakeRandomItem(rules, weights, random, out var ruleObjectOut); // take random rule from the available ones
                    //var ruleObject = ruleObjectOut as SH_Rule;

                    // for the rule to be added, it must comply with the current state. 
                    var copyRule = ruleObject.CopyRule(ruleObject);
                    if (ruleObject.RuleState == state)
                    {



                        // Remove the below line to see if it works using simpleShape instead
                        /*
                        // test if it is SH_Rule02
                        var r2 = ruleObject as SH_Rule02;
                        if (r2 != null)
                        {
                            numLines++; // if true, then a new line can be added to the SH_Simple shape from this rule.
                        }*/
                        try
                        {
                            copyRule.NewRuleParameters(random, simpleShape);
                            genome.Add(copyRule); // only add the rule if no exception come up.

                            try
                            {
                                string message = copyRule.RuleOperation(ref simpleShape);

                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Something went wrong.");
                            }
                            // change the state to the one applied by the rule
                            state = copyRule.GetNextState();
                        }
                        catch (Exception e)
                        {
                            state = simpleShape.SimpleShapeState;
                            // Could log a message here. 
                        }
                        
                        //var copyRule = ruleObject.CopyRule(ruleObject); // this is not working. Need to make it serializable// add the rule to the genome
                         //  the latest rule object seem to override all the previously entered rules. 

                        // modify the copied shape with the latest rule
                        

                        
                    }




                    

                    count++;
                }
                else
                {
                    throw new Exception("There was an error when taking the element from the list");
                }

                /*
                // apply new rule until the final rule is selected.
                while (state != State.end)
                {
                    var randomRule = 
                }
                */
                // assign the modified shape as a parameter to the variabel we're evaluating. 




                variable.SimpleShape = simpleShape;
            }
            
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
            instantiateObject.NewRuleParameters(rnd, new SH_SimpleShape()); // Set the translation vectors for this

            return instantiateObject;
        }
        public static object NewRule(string ruleStr)
        {
            // Make this more general to work with new rules as well.
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
