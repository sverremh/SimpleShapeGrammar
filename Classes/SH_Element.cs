using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShapeGrammar.Classes
{
    [Serializable]
    public class SH_Element
    {
        // --- properties ---
        public static int IDCounter { get; set; };
        public int ID { get; }
        // public int ID { get; }
        public SH_Node[] Nodes { get; }
        // --- constructors ---
        public SH_Element(SH_Node[] _nodes) 
        {
            //SH_UtilityClass.LineCount += 1;
            //ID = SH_UtilityClass.LineCount;
            ID = ++IDCounter;
            Nodes = _nodes;
        }


        // --- methods ---

    }
}
