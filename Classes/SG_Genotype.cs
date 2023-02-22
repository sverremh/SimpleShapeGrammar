using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGrammar.Classes
{
    public class SG_Genotype
    {
        // --- properties ---
        public List<int> IntGenes { get; set; } = new List<int>();
        public List<double> DGenes { get; set; } = new List<double>();

        // --- constructors ---

        public SG_Genotype()
        { 
        }
        public SG_Genotype(List<int> _ints, List<double> _ds)
        {
            IntGenes = _ints;
            DGenes = _ds;
        }

        // --- methods ---

        public List<double> Export()
        {
            List<double> exportLst = new List<double>() { IntGenes.Count, DGenes.Count };
            exportLst.AddRange(IntGenes.Select(n => (double) n).ToList());
            exportLst.AddRange(DGenes);

            return exportLst;
        }

    }
}
