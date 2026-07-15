using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Librerías de datos, controlador de SQLite y configuraciones de la aplicación
using System.Data;
using System.Data.SQLite;
using System.Configuration;

namespace SisBecas
{
    public static class DbHelper
    {
        // Obtiene la cadena de conexión guardada en el archivo App.config
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ConexionBecas"].ConnectionString;
        }

        // Retorna un nuevo objeto de conexión configurado con nuestra ruta de base de datos
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(GetConnectionString());
        }

        // Obtiene todos los tipos de beca activos para llenar la lista desplegable
        public static DataTable ObtenerTiposBeca()
        {
            DataTable dt = new DataTable();
            string query = "SELECT id_tipo, nombre, monto_mensual FROM tipos_beca WHERE activo = 1 ORDER BY id_tipo ASC";

            using (SQLiteConnection conn = GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        da.Fill(dt); // Llena la tabla en memoria con los resultados de la consulta
                    }
                }
            }
            return dt;
        }

        // Verifica si ya existe un aspirante con el mismo número de DUI
        public static bool ExisteDUI(string dui)
        {
            string query = "SELECT COUNT(*) FROM aspirantes WHERE dui = @dui";
            using (SQLiteConnection conn = GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@dui", dui);
                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        // Inserta los datos del formulario de inscripción en la tabla de aspirantes
        public static bool InsertarAspirante(
    string dui, string nombres, string apellidos, DateTime fechaNac,
    string sexo, string telefono, string correo, string institucion,
    decimal promedio, decimal ingreso, int idTipo)
        {
            // Corregido: "fecha_nacimiento = @fecha_nac" en lugar del texto erróneo anterior
            string query = @"INSERT INTO aspirantes 
        (dui, nombres, apellidos, fecha_nacimiento, sexo, telefono, correo, institucion_estudio, promedio, ingreso_familiar, id_tipo) 
        VALUES 
        (@dui, @nombres, @apellidos, @fecha_nac, @sexo, @telefono, @correo, @institucion, @promedio, @ingreso, @idTipo)";

            using (SQLiteConnection conn = GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    // Asignación y mapeo seguro de cada parámetro de entrada hacia la consulta SQL
                    cmd.Parameters.AddWithValue("@dui", dui);
                    cmd.Parameters.AddWithValue("@nombres", nombres);
                    cmd.Parameters.AddWithValue("@apellidos", apellidos);
                    cmd.Parameters.AddWithValue("@fecha_nac", fechaNac.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@sexo", sexo);
                    cmd.Parameters.AddWithValue("@telefono", string.IsNullOrWhiteSpace(telefono) ? (object)DBNull.Value : telefono);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@institucion", institucion);
                    cmd.Parameters.AddWithValue("@promedio", promedio);
                    cmd.Parameters.AddWithValue("@ingreso", ingreso);
                    cmd.Parameters.AddWithValue("@idTipo", idTipo);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery(); // Ejecuta la inserción y retorna el número de registros guardados
                    return rows > 0;
                }
            }
        }

        // Busca la información completa de un aspirante en base a su número de DUI
        public static DataTable ObtenerAspirantePorDUI(string dui)
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM aspirantes WHERE dui = @dui";
            using (SQLiteConnection conn = GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@dui", dui);
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        da.Fill(dt); // Guarda los datos encontrados en la tabla en memoria
                    }
                }
            }
            return dt;
        }
    }
}
