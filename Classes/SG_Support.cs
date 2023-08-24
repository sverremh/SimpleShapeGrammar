﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
namespace ShapeGrammar.Classes
{
    [Serializable]
    public class SG_Support
    {
        // --- properties ---
        
        public int ID;
        // public int nodeInd;
        public SG_Node Node { get; set; }
        public Point3d Pt { get; set; }
        public int SupportCondition { get; set; }

        // --- constructors ---

        public SG_Support()
        {

        }
        public SG_Support(string _sup_cond_txt, Point3d _pt) //  SG_Node _node
        {
            // Test if the support condition are in the correct format
            if (_sup_cond_txt.Length != 6) throw new Exception("The length of the string must be exactly 6 characters");

            //Test if the elements in the string are correct
            foreach (char item in _sup_cond_txt)
            {
                if (item != '0' && item != '1')
                {
                    throw new Exception("Only 0 and 1 should be present in the string");
                }
            }

            // Create the support condition 
            SupportCondition = SetConditions(_sup_cond_txt);
            Pt = _pt;
            
        }
        // --- methods ---
        private int SetConditions(string _stringCondition)
        {
            int condition = 0;
            int n = 0;
            foreach (char el in _stringCondition)
            {
                if (el == '1')
                {
                    condition += (int) Math.Pow(2, n);
                }
                n++; 
            }
            return condition;            
        }

    }
}
