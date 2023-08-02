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
    internal class NReceta
    {
        public static List<Recetas> ObtenerReceta(int id)
        {
            try
            {
                DReceta data = new DReceta();
                List<Recetas> listReceta = new List<Recetas>();

                var listPoints = DReceta.GetReceta(id);
                listReceta = (from DataRow i in listPoints.Rows
                              select new Recetas()
                              {
                                  Area = Convert.ToInt32(i["Area"]),
                                  SecondArea = Convert.ToInt32(i["AreaB"])
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
