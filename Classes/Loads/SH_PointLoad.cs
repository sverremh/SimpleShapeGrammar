using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace ShapeGrammar.Classes
{
    [Serializable]
    public class SH_PointLoad : SH_Load
    {
        // --- properties ---
        public Vector3d Forces { get; set; }
        public Vector3d Moments { get; set; }
        public Point3d Position { get; set; }
        // --- constructors --
        public SH_PointLoad()
        {
            // empty
        }
        public SH_PointLoad(Vector3d _forces, Vector3d _moments, Point3d _position)
        {
            Forces = _forces;
            Moments = _moments;
            Position = _position;
        }
        // --- methods ---
    }
}
