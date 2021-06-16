using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
namespace SimpleShapeGrammar.Classes
{
    [Serializable]
    public class SH_Support
    {
        // --- properties ---
        public SH_Node node;
        public Point3d Position { get; set; }
        public int SupportCondition { get; set; }

        // --- constructors ---
        public SH_Support(string _support_conditions, Point3d _position)
        {
            // Test if the support condition are in the correct format
            if (_support_conditions.Length != 6) throw new Exception("The length of the string must be exactly 6 characters");
            //Test if the elements in the string are correct
            foreach (char item in _support_conditions)
            {
                if (item != '0' && item != '1')
                {
                    throw new Exception("Only 0 and 1 should be present in the string");
                }
            }

            // Create the support
            SupportCondition = SetConditions(_support_conditions);

            Position = _position;
            
        }
        // --- methods ---
        private int SetConditions(string _stringCondition)
        {
            int condition = 0;
            int n = 0;
            foreach (char el in _stringCondition)
            {
                if (el == 1)
                {
                    condition += (int) Math.Pow(2, n);
                }
                n++; 
            }
            return condition;            
        }
    }
}
