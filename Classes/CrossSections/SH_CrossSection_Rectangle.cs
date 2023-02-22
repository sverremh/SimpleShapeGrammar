using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGrammar.Classes
{
    [Serializable]
    class SH_CrossSection_Rectangle : SH_CrossSection_Beam
    {
        // --- properties ---        
        public double height;
        public double width;
       
        // --- constructors --
        public SH_CrossSection_Rectangle()
        {
            // empty
        }
        public SH_CrossSection_Rectangle(string _name, double _height, double _width)
        {
            Name = _name;
            height = _height;
            width = _width;
            //Area = _height * _width;
            Wy = (_width * Math.Pow(_height, 2)) / 6;
            Iy = (_width * Math.Pow(_height, 3)) / 12;
            Area = width * height;
        }

        

        // --- methods ---
        public double GetCrossSectionWeigth()
        {
            return Area* Material.Density;
        }
            
    }
}
