using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShapeGrammar.Classes
{
    [Serializable]
    public abstract class SH_Rule
    {

        public SH_Rule()
        { 
            
        }
        public abstract string RuleOperation(SH_SimpleShape _ss);
        
        
    }


}
