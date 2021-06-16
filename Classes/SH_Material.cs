using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShapeGrammar.Classes
{
    [Serializable]
    public class SH_Material
    {
        
        // --- properties ---
        public string Tag { get; private set; }
        public string Name { get; private set; }
        public double Density { get; private set;  }
        public double E { get; private set; }
        public double G { get; private set; }
        public double Fy { get; private set; }
        public double Poisson { get; private set; }

        // --- constructors --
        public SH_Material()
        {

        }
        public SH_Material(string _name, double _e, double _v, double _fy, double _rho)
        {
            Name = _name;
            Density = _rho;
            E = _e;
            Poisson = _v;
            Fy = _fy;
            G = _e / (2 * (1 + _v) );
            

        }
        // --- methods ---
    }
}
