using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace SimpleShapeGrammar.Classes.Elements
{
    [Serializable]
    public class SH_Surface : SH_Element
    {
        // -- properties -- 
        public Surface elementSurface { get; set; }

        // -- constructors --
        public SH_Surface()
        {
            // empty constructor
        }

        public SH_Surface(Surface elementSurface)
        {
            this.elementSurface = elementSurface;
        }

        public SH_Surface(Surface elementSurface, string _name)
        {
            this.elementSurface = elementSurface;
            elementName = _name;
        }

        public SH_Surface(string _name, int _id, Surface _surface)
        {
            elementName = _name;
            ID = _id;
            elementSurface = _surface;
        }
        
        // -- methods --
    }
}
