using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRACKING.Entity
{
    public class Referencias
    {
        public long Id { get; set; }
        public long IdReceta { get; set; }

        public int distUmbralBelow { get; set; }
        public int distUmbralAbove { get; set; }
        public int Umbral_Threshold { get; set; }
        public int kernel_hor { get; set; } 
        public int Kernel__ver { get; set; }
        public int Kerne_cir_chor { get; set; }
        public int kernel_circ_ver { get; set; }
        public int width_umbral { get; set; }
        public int height_umbral { get; set; }
        public int contraste_mult { get; set; }
        public int contraste_sun { get; set; }

        public double porcentaje_umbral { get; set; }

    }
}
