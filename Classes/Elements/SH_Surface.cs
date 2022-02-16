﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

using Rhino.Geometry;
namespace SimpleShapeGrammar.Classes.Elements
{
    public class SH_Surface : SH_Element
    {
        // -- properties -- 
        private Surface elementSurface { get; set; }

        // -- constructors --
        public SH_Surface()
        {
            // empty constructor
        }
        public SH_Surface(Surface elementSurface)
        {
            this.elementSurface = elementSurface;
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