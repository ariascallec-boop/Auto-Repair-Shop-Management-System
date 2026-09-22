using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class NuevoServicioModel : PageModel
    {
        private readonly MySqlConnection _connection;

        public NuevoServicioModel(MySqlConnection connection)
        {
            _connection = connection;
        }

        [BindProperty]
        public Servicio Servicio { get; set; } = new Servicio();

        public void OnGet()
        {
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
                Servicio.Estado = true;

                string query = @"INSERT INTO servicio (nombre, tipo, descripcion, precio, duracion_estimada, unidad_duracion, estado)
                         VALUES (@nombre, @tipo, @descripcion, @precio, @duracionEstimada, @unidadDuracion, @estado);";

                _connection.Open();

                using MySqlCommand command = new MySqlCommand(query, _connection);

                command.Parameters.AddWithValue("@nombre", Servicio.Nombre);
                command.Parameters.AddWithValue("@tipo", Servicio.Tipo);
                command.Parameters.AddWithValue("@descripcion", Servicio.Descripcion);
                command.Parameters.AddWithValue("@precio", Servicio.Precio);
                command.Parameters.AddWithValue("@duracionEstimada", Servicio.DuracionEstimada);
                command.Parameters.AddWithValue("@unidadDuracion", Servicio.UnidadDuracion);
                command.Parameters.AddWithValue("@estado", Servicio.Estado);

                command.ExecuteNonQuery();

                return RedirectToPage("/Servicios");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar el servicio: " + ex.Message);
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