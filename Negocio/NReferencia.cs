using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRACKING.Data;
using TRACKING.Entity;

namespace TRACKING.Negocio
{
    public class NReferencia
    {
        public static List<Referencias> ObtenerReferenciaA(int id)
        {
            try
            {
                DReferencia data = new DReferencia();
                List<Referencias> listReceta = new List<Referencias>();

                var listPoints = DReferencia.GetReferenciaA(id);
                listReceta = (from DataRow i in listPoints.Rows
                              select new Referencias()
                              {
                                  Id = (long)i["Id"],
                                  distUmbralBelow = Convert.ToInt32(i["dist umbral below"]),
                                  distUmbralAbove = Convert.ToInt32(i["dis umbral above"]),
                                  Umbral_Threshold = Convert.ToInt32(i["Umbral Threshold"]),
                                  kernel_hor = Convert.ToInt32(i["kernel hor"]),
                                  Kernel__ver = Convert.ToInt32(i["Kernel  ver"]),
                                  Kerne_cir_chor = Convert.ToInt32(i["Kerne cir hor"]),
                                  kernel_circ_ver = Convert.ToInt32(i["kernel cir ver"]),
                                  width_umbral = Convert.ToInt32(i["width umbral"]),
                                  height_umbral = Convert.ToInt32(i["height umbral"]),
                                  contraste_mult = Convert.ToInt32(i["contraste mult"]),
                                  contraste_sun = Convert.ToInt32(i["contraste sun"]),
                                  porcentaje_umbral = Convert.ToDouble(i["porcentaje umbral"]),

                              }).ToList();
                return listReceta;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public static List<Referencias> ObtenerReferenciaB(int id)
        {
            try
            {
                DReferencia data = new DReferencia();
                List<Referencias> listReceta = new List<Referencias>();

                var listPoints = DReferencia.GetReferenciaB(id);
                listReceta = (from DataRow i in listPoints.Rows
                              select new Referencias()
                              {
                                  Id = (long)i["Id"],
                                  distUmbralBelow = Convert.ToInt32(i["dist umbral below"]),
                                  distUmbralAbove = Convert.ToInt32(i["dis umbral above"]),
                                  Umbral_Threshold = Convert.ToInt32(i["Umbral Threshold"]),
                                  kernel_hor = Convert.ToInt32(i["kernel hor"]),
                                  Kernel__ver = Convert.ToInt32(i["Kernel  ver"]),
                                  Kerne_cir_chor = Convert.ToInt32(i["Kerne cir hor"]),
                                  kernel_circ_ver = Convert.ToInt32(i["kernel cir ver"]),
                                  width_umbral = Convert.ToInt32(i["width umbral"]),
                                  height_umbral = Convert.ToInt32(i["height umbral"]),
                                  contraste_mult = Convert.ToInt32(i["contraste mult"]),
                                  contraste_sun = Convert.ToInt32(i["contraste sun"]),
                                  porcentaje_umbral = Convert.ToDouble(i["porcentaje umbral"]),
                              }).ToList();
                return listReceta;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}
