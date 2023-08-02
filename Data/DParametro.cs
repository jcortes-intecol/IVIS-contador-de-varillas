using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRACKING.Data
{
    internal class DParametro
    {
        public static String ObtenerParametro(string parameter)
        {
            string Rpta = "";
            SqlConnection sqlcon = new SqlConnection();
            try
            {
                sqlcon = Conexion.getInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("[Configuracion].[usp_ObtenerParametro]", sqlcon);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@Parametro", SqlDbType.VarChar).Value = parameter;
                sqlcon.Open();
                //use.ExecuteScalar() in case you need to query data
                Rpta = (string)Comando.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Rpta = ex.Message;
            }
            finally
            {
                if (sqlcon.State == ConnectionState.Open) sqlcon.Close();
            }
            return Rpta;

        }

        public static void ActualizarParametro(string parametro, string tipo, string valor)
        {

            string Rpta = "";
            SqlConnection sqlcon = new SqlConnection();
            try
            {
                sqlcon = Conexion.getInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("[Configuracion].[usp_ActualizarParametro]", sqlcon);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@Parametro", SqlDbType.VarChar).Value = parametro;
                Comando.Parameters.Add("@Tipo", SqlDbType.VarChar).Value = tipo;
                Comando.Parameters.Add("@Valor", SqlDbType.VarChar).Value = valor;

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

