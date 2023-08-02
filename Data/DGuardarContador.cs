using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRACKING.Data
{
    public class DGuardarContador
    {
        public static void GuardarContador(int IdReceta, string contador)
        {

            string Rpta = "";
            SqlConnection sqlcon = new SqlConnection();
            try
            {
                sqlcon = Conexion.getInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("[Operacion].[usp_CrearMedicion]", sqlcon);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@Contador", SqlDbType.BigInt).Value = contador;
                Comando.Parameters.Add("@IdReceta", SqlDbType.BigInt).Value = IdReceta;

                sqlcon.Open();

                Comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Rpta = ex.Message;
            }
            finally
            {
                if (sqlcon.State == ConnectionState.Open) sqlcon.Close();
            }


        }

        public static void GuardarContadorB(int IdReceta, string contador)
        {

            string Rpta = "";
            SqlConnection sqlcon = new SqlConnection();
            try
            {
                sqlcon = Conexion.getInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("[Operacion].[usp_CrearMedicionB]", sqlcon);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@Contador", SqlDbType.BigInt).Value = contador;
                Comando.Parameters.Add("@IdReceta", SqlDbType.BigInt).Value = IdReceta;

                sqlcon.Open();

                Comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Rpta = ex.Message;
            }
            finally
            {
                if (sqlcon.State == ConnectionState.Open) sqlcon.Close();
            }


        }
    }
}
