using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoScrum.Clases;
namespace TallerMecanicoScrum.Pages
{
    public class ServiciosModel : PageModel
    {
        private readonly MySqlConnection _connection;

        public ServiciosModel(MySqlConnection connection)
        {
            _connection = connection;
        }
        public List<Servicio> ListaServicios { get; set; } = new List<Servicio>();

        public string Buscar { get; set; } = "";

        public void OnGet(string buscar)
        {
            Buscar = buscar ?? "";

            if (string.IsNullOrWhiteSpace(Buscar))
            {
                CargarServicios();
            }
            else
            {
                BuscarServicios();
            }
        }

        private void CargarServicios()
        {

            try
            {
                string query = @"SELECT id,nombre, tipo, descripcion, precio, duracion_estimada, unidad_duracion, 
                                estado FROM servicio WHERE estado = TRUE ORDER BY nombre; ";
                _connection.Open();
                using MySqlCommand command = new MySqlCommand(query, _connection);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Servicio servicio = new Servicio
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Nombre = reader["nombre"].ToString() ?? "",
                        Tipo = reader["tipo"].ToString() ?? "",
                        Descripcion = reader["descripcion"].ToString() ?? "",
                        Precio = Convert.ToDecimal(reader["precio"]),
                        DuracionEstimada = Convert.ToInt32(reader["duracion_estimada"]),
                        UnidadDuracion = reader["unidad_duracion"].ToString() ?? "",
                        Estado = Convert.ToBoolean(reader["estado"])
                    };
                    ListaServicios.Add(servicio);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar servicios: " + ex.Message);
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

        }

        private void BuscarServicios()
        {

            try
            {
                string query = @"SELECT id, nombre, tipo, descripcion, precio, duracion_estimada,unidad_duracion, estado 
                                FROM servicio WHERE estado = TRUE  ";

                if (!string.IsNullOrWhiteSpace(Buscar))
                {
                    query += @"
                AND ( nombre LIKE @buscar OR tipo LIKE @buscar)  ";
                }
                query += " ORDER BY nombre;";

                _connection.Open();
                using MySqlCommand command = new MySqlCommand(query, _connection);
                {
                    if (!string.IsNullOrWhiteSpace(Buscar))
                    {
                        command.Parameters.AddWithValue("@buscar", "%" + Buscar + "%" );
                       
                    }
                    using MySqlDataReader reader = command.ExecuteReader();
                    {
                        while (reader.Read())
                        {
                            Servicio servicio = new Servicio
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Nombre = reader["nombre"].ToString() ?? "",
                                Tipo = reader["tipo"].ToString() ?? "",
                                Descripcion = reader["descripcion"].ToString() ?? "",
                                Precio = Convert.ToDecimal(reader["precio"]),
                                DuracionEstimada =
                                    Convert.ToInt32(reader["duracion_estimada"]),
                                UnidadDuracion =
                                    reader["unidad_duracion"].ToString() ?? "",
                                Estado = Convert.ToBoolean(reader["estado"])
                            };

                            ListaServicios.Add(servicio);
                        }

                    }

                }
            }
            catch
            {

                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        public IActionResult OnPostEliminar(int id)
        {
            try
            {
                string query = @" UPDATE servicio SET estado = FALSE WHERE id = @id; ";

                _connection.Open();

                using MySqlCommand command = new MySqlCommand(query, _connection);

                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
            catch 
            {
                throw;
            }
            finally
            {
                if (_connection.State ==
                    System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return RedirectToPage("/Servicios");
        }
    }
}