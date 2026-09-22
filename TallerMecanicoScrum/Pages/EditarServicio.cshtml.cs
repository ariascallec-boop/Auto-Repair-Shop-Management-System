using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class EditarServicioModel : PageModel
    {
        private readonly MySqlConnection _connection;

        public EditarServicioModel(MySqlConnection connection)
        {
            _connection = connection;
        }

        [BindProperty]
        public Servicio Servicio { get; set; } = new Servicio();

        public void OnGet(int id)
        {
            try
            {
                string query = @"SELECT id, nombre, tipo, descripcion, precio, duracion_estimada, unidad_duracion, estado
                                 FROM servicio WHERE id = @id;";

                _connection.Open();

                using MySqlCommand command = new MySqlCommand(query, _connection);

                command.Parameters.AddWithValue("@id", id);

                using MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Servicio = new Servicio
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar servicio: " + ex.Message);
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        public IActionResult OnPost()
        {
            Servicio.Nombre = Validaciones.Capitalizar(Servicio.Nombre);

            if (!Validaciones.LetrasNumerosEspacios(Servicio.Nombre))
            {
                ModelState.AddModelError(
                    "Servicio.Nombre",
                    "El nombre solo puede contener letras, números y espacios"
                );
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                string query = @"UPDATE servicio SET nombre=@nombre, tipo=@tipo, descripcion=@descripcion, precio=@precio,
                                 duracion_estimada=@duracionEstimada, unidad_duracion=@unidadDuracion WHERE id=@id;";

                _connection.Open();

                using MySqlCommand command = new MySqlCommand(query, _connection);

                command.Parameters.AddWithValue("@nombre", Servicio.Nombre);
                command.Parameters.AddWithValue("@tipo", Servicio.Tipo);
                command.Parameters.AddWithValue("@descripcion", Servicio.Descripcion);
                command.Parameters.AddWithValue("@precio", Servicio.Precio);
                command.Parameters.AddWithValue("@duracionEstimada", Servicio.DuracionEstimada);
                command.Parameters.AddWithValue("@unidadDuracion", Servicio.UnidadDuracion);
                command.Parameters.AddWithValue("@id", Servicio.Id);

                command.ExecuteNonQuery();

                return RedirectToPage("/Servicios");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Error al actualizar el servicio: " + ex.Message
                );

                return Page();
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }
    }
}