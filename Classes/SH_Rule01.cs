using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SimpleShapeGrammar.Classes
{
    public class SH_Rule01 : SH_Rule
    {
        // --- properties ---
        public double P0X { get; private set; }
        public double P0Y { get; private set; }
        public double P0Z { get; private set; }
        public double P1X { get; private set; }
        public double P1Y { get; private set; }

        public double P1Z { get; private set; }

        // --- constructors ---
        public SH_Rule01()
        {
            
        }

        // --- methods ---
        void RuleOperation(SH_SimpleShape _ss)
        { 
            // take the 1st element
            // SH_Line sh_line = _ss
        }

    }
}
