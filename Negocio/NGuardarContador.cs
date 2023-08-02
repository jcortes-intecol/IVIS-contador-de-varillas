using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRACKING.Data;

namespace TRACKING.Negocio
{
    internal class NGuardarContador
    {
        public static void GuardarContador(int id, string cont)
        {
            try
            {
                DGuardarContador.GuardarContador(id, cont);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }
        public static void GuardarContadorB(int id, string cont)
        {
            try
            {
                DGuardarContador.GuardarContadorB(id, cont);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

    }
}

