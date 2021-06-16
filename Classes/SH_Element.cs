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
        
        public int? ID { get; set; }
        // public int ID { get; }
        public SH_Node[] Nodes { get; }

        public SH_CrossSection CrossSection { get; set; }

        // --- constructors ---
        public SH_Element(SH_Node[] _nodes, int? _id) 
        {
            
            ID = _id;
            Nodes = _nodes;
        }


        // --- methods ---

    }
}
