using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace ShapeGrammar.Classes
{
    [Serializable]
    public class SH_LineLoad : SH_Load
    {
        // --- properties ---
        public string ElementId{ get; set; }
        public int LoadCase { get; set; }
        public Vector3d Load { get; set; }

        // --- constructors ---
        public SH_LineLoad()
        {
            // empty
        }

        public SH_LineLoad(int _loadCase, Vector3d _loaddirection)
        {
            LoadCase = _loadCase;
            Load = _loaddirection;
        }

        // --- methods ---
    }
}
