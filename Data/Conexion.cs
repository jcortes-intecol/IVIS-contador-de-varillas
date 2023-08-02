using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRACKING.Data
{
    internal class Conexion
    {
        private static Conexion ConexionDB_aux = null;

        private Conexion() //constructor
        {


        }
        public SqlConnection CrearConexion()
        {
            SqlConnection Comando = new SqlConnection();
            try
            {
                //Comando.ConnectionString = "Server=" + this.Serividor + "; Database =" + this.DataBase + ";";
                //if (this.Seguridad)
                //{
                //    Comando.ConnectionString = Comando.ConnectionString + "Integrated Security = SSPI";
                //}
                //else{
                //    Comando.ConnectionString = Comando.ConnectionString + "User Id =" + this.Usuario + "; Password =" + this.Clave;
                //}

                Comando.ConnectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=IvisTernium;Integrated Security=True";

                //Comando.ConnectionString = "Data Source=DESKTOP-NMMMEK1\\SQLEXPRESS;Initial Catalog=MetroCotas;Integrated Security=True";

            }
            catch (Exception ex)
            {
                Comando = null;
                throw ;
            }
            return Comando;

        }
        public static Conexion getInstancia()
        {
            if (ConexionDB_aux == null)
            {
                ConexionDB_aux = new Conexion();
            }
            return ConexionDB_aux;
        }
    }
}

