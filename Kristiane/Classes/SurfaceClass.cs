using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShapeGrammar.Kristiane.Classes
{
    public class SurfaceClass
    {
        //propterties
        public string Name { get; set; }
        public double Srf { get; set; }

        //constructor
        public SurfaceClass()
        {
            //empty
        }

        public SurfaceClass(double _surface, string _name)
        {
            Srf = _surface;
            Name = _name;
        }

    }
}
