using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

namespace ShapeGrammar.Classes.Elements
{
    [Serializable]
    public class SH_Solid : SH_Element
    {
        // -- properties --
        public Brep Brep { get; set; }

        // -- constructors --
        public SH_Solid()
        {
            // empty constructor
        }

        public SH_Solid(string _name, int _id)
        {
            elementName = _name;
            ID = _id;
        }

        public SH_Solid(string _name, int _id, Brep _brep)
        {
            elementName = _name;
            ID = _id;
            Brep = _brep;
        }
        public SH_Solid(string _name, Brep _brep)
        {
            elementName = _name;
            Brep = _brep;
        }



        // -- methods


    }
}
