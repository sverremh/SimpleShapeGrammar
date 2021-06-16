using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShapeGrammar.Classes
{
    [Serializable]
    public class SH_CrossSection
    {
        // --- properties ---
        public string Tag { get; set;  }
        public string Name { get; set; }
        public SH_Material Material { get; set; }
        public double Wy { get; internal set; }
        public double Iy { get; internal set; }
        public double Area { get; internal set; }

        // --- constructors --
        
        // --- methods ---
    }
}
