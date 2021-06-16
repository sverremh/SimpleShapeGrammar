using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShapeGrammar.Classes
{
    [Serializable]
    class SH_Rectangle: SH_CrossSection
    {
        // --- properties ---        
        public double heigth;
        public double width;
       
        // --- constructors --
        public SH_Rectangle()
        {
            // empty
        }
        public SH_Rectangle(string _name, double _height, double _width)
        {
            Name = _name;
            heigth = _height;
            width = _width;
            Area = _height * _width;
            Wy = (_width * Math.Pow(_height, 2)) / 6;
            Iy = (_width * Math.Pow(_height, 3)) / 12;
        }

        // --- methods ---
        public double GetCrossSectionWeigth()
        {
            return Area* Material.Density;
        }
            
    }
}
