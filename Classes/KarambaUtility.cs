using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karamba.Geometry;
using Rhino.Geometry;

using Karamba.GHopper;
using Karamba.GHopper.Geometry;
using Karamba.Utilities.Mesher;

namespace SimpleShapeGrammar.Classes
{
    /// <summary>
    /// Class for general operations using Karamba
    /// </summary>
    //[Serializable] Not sure if this is needed
    public static class KarambaUtility
    {
        /// <summary>
        /// Meshes a surface using a similar approach as the "MeshBreps" Karamba component. 
        /// </summary>
        /// <param name="srf"></param>
        /// <returns>Mesh which can be used for applying loads and creating shell elements.</returns>
        public static Mesh3 MeshSurface(Surface srf, List<Point3d> inclusionPoint3ds, double horSizeTol, double inclusionRadius, double scalarFactor, double gradation, double tol)
        {
            /// Why is this not working from C# script component. Seems to be the mesh() method that is causing the problem. 
            GHMesh m = new GHMesh();
            
            List<Point3> incList = (inclusionPoint3ds).Select(p => p.Convert()).ToList<Point3>();
            if (srf!= null)
            {
                return m.mesh((IBrep)new RhinoBrep(srf.ToBrep()), (IEnumerable<Point3>)incList, horSizeTol,
                    inclusionRadius, scalarFactor, gradation, tol);
            }
            else
            {
                return null;
            }
        }
        public static IBrep ConvertToRhinoBrep(Surface srf)
        {
            return (IBrep)new RhinoBrep(srf.ToBrep());
        }

    }
}
