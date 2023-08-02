using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRACKING.Data
{
    internal class DReceta
    {
        public static DataTable GetReceta(int id)
        {

            SqlDataReader Result;
            DataTable Table = new DataTable();
            SqlConnection sqlcon = new SqlConnection();
            try
            {
                sqlcon = Conexion.getInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("[Configuracion].[usp_ObtenerReceta]", sqlcon);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@Id", SqlDbType.BigInt).Value = id;
                sqlcon.Open();
                //use.ExecuteScalar() in case you need to query data
                Result = Comando.ExecuteReader();
                Table.Load(Result);
                return Table;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (sqlcon.State == ConnectionState.Open) sqlcon.Close();
            }

        }
    }
}
