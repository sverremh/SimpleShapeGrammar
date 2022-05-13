using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace SimpleShapeGrammar.Classes.Loads
{
    [Serializable]
    public class SH_SurfaceLoad : SH_Load
    {
        public Vector3d AreaLoad { get; set; }

        public SH_SurfaceLoad(Vector3d loadVec, int loadCase)
        {
            LoadCase = loadCase;
            AreaLoad = loadVec;
        }
    }
}
