using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShapeGrammar.Classes
{
    public class SH_Line
    {
        // --- properties ---
        public int ID { get; }
        public SH_Node[] nodes;
        // --- constructors ---
        public SH_Line(SH_Node[] _nodes) 
        {
            SH_UtilityClass.LineCount += 1;
            ID = SH_UtilityClass.LineCount;
            nodes = _nodes;
        }


        // --- methods ---

    }
}
