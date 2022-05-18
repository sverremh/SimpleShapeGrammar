using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Accord;
//using CsvHelper;
using Grasshopper.Kernel.Types;
using Karamba.Geometry;
using Rhino.Geometry;

using Karamba.GHopper;
using Karamba.GHopper.Geometry;
using Karamba.GHopper.Utilities;
using Karamba.Materials;
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
        /// Create necessary description. 
        /// </summary>
        /// <param name="srf"></param>
        /// <param name="inclusionPoint3ds"></param>
        /// <param name="mResolution"></param>
        /// <param name="edgeRefinement"></param>
        /// <param name="inclusionRadius"></param>
        /// <param name="scalarFactor"></param>
        /// <param name="gradation"></param>
        /// <param name="info"></param>
        /// <param name="cullDist"></param>
        /// <param name="mesh_mode"></param>
        /// <param name="tol"></param>
        /// <returns>A mesh representation of the input brep.</returns>
        public static Mesh MeshSurface(Surface srf, List<Point3d> inclusionPoint3ds, double mResolution,double edgeRefinement,  double inclusionRadius, double scalarFactor, double gradation, out string info, double cullDist = 1.0, int mesh_mode = 1, double tol = 0.01)
        {
            // This method is working.Should use this in the future. Then convert to Mesh3...
            Karamba.GHopper.Utilities.MeshBreps.solve(new List<Brep>() { srf.ToBrep() }, inclusionPoint3ds, mResolution, mesh_mode,
                edgeRefinement, cullDist, tol, 1.0, 4, out string text, out info, out List<Mesh> resultMeshes);
            return resultMeshes[0];
            /*
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
            }*/
        }
        /// <summary>
        /// Converts a Surface to a IBrep interface from KarambaCommon. 
        /// </summary>
        /// <param name="srf">The instance of the Rhino Surface class</param>
        /// <returns>An instance of an IBrep from KarambaCommon</returns>
        public static IBrep ConvertToRhinoBrep(Surface srf)
        {
            return (IBrep)new RhinoBrep(srf.ToBrep());
        }

        public static List<Mesh3> MeshToMesh3(List<Mesh> mLst)
        {
            var mg = mLst.Select(m => new GH_Mesh(m)).ToList(); // convert to GH_Mesh
            var m3 = FromGH.Values(mg); // convert to Karamba Mesh
            return m3;

        }

        public static List<string> GetKarambaMaterial(string materialName)
        {
            string dir0 = Directory.GetCurrentDirectory(); // get the directory for later. 
            Directory.SetCurrentDirectory(@"$(UserProfile)\AppData\Roaming\Grasshopper\7\Libraries\Karamba\Materials\");
            var filePath = "MaterialProperties_2_2_0.csv";
            Directory.SetCurrentDirectory(dir0); // reset the directory back to default
            /*
            var streamReader = File.OpenText(filePath);
            var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            var test = csvReader.GetRecords<string>().ToList();

            streamReader.Close(); // always close the file. 

            return test[0];

           */

            var materialData = new List<string>();
            using (var reader = new StreamReader(filePath))
            {
                
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.Contains(materialName))
                    {
                        materialData.Add(line);
                    }
                }
            }

            return materialData;

        }
    }
}
