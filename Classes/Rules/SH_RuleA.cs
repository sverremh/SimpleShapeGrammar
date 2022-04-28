﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShapeGrammar.Classes
{
    [Serializable]
    public class SH_RuleA : SH_Rule
    {
        // --- properties ---
        //public State RuleState = State.beta;

        // --- constructors --
        public SH_RuleA()
        {
            // empty
            RuleState = State.beta;
            Name = "SH_RuleA";
        }

        public override void NewRuleParameters(Random random, SH_SimpleShape ss)
        {

        }

        public override SH_Rule CopyRule(SH_Rule rule)
        {
            throw new NotImplementedException();
        }

        // --- methods ---
        public override string RuleOperation(SH_SimpleShape _ss)
        {
            // check if the state maches the simple shape state
            if (_ss.SimpleShapeState != RuleState)
            {
                return "RuleA is not applicable with the current state.";
            }

            // change the state
            _ss.SimpleShapeState = State.gamma;
            return "RuleA applied!";
        }
        public override State GetNextState()
        {
            return State.gamma;
        }
    }
}
