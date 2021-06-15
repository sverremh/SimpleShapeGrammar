using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShapeGrammar.Classes
{
    class SH_SimpleShape
    {
        // --- properties ---
        public List<SH_Line> Lines { get; set; }

        public List<SH_Node> Nodes { get; set; }

        
        
        
        // --- constructors ---
        public SH_SimpleShape()
        {
            // empty constructor
        }

        // --- methods ---
        public void AddLine(SH_Line _line)
        {
            Lines.Add(_line);
        }
    }
}
