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
        private int ID { get; }
        public SH_Node[] nodes;
        // --- constructors ---
        public SH_Line(int _id, SH_Node[] _nodes) 
        {
            ID = _id;
            nodes = _nodes;
        }


        // --- methods ---

    }
}
