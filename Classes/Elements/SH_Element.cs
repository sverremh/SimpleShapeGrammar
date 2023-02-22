using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Rhino.Geometry;
namespace ShapeGrammar.Classes.Elements
{
    [Serializable] 
    public abstract class SH_Element //: SH_CrossSection_Beam
    {
        // --- properties ---
        
        public int? ID { get; set; }
        public string Name { get; set; }

        public SH_Node[] Nodes { get; set; }

    }
}
