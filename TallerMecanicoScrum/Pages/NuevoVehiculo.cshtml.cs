using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class NuevoVehiculoModel : PageModel
    {
        private readonly MySqlConnection _connection;

        public NuevoVehiculoModel(MySqlConnection connection)
        {
            _connection = connection;
        }

        [BindProperty]
        public Vehiculo Vehiculo { get; set; } = new Vehiculo();

        public List<SelectListItem> ListaClientes { get; set; } = new List<SelectListItem>();

        public void OnGet()
        {
            CargarClientes();
        }

        public IActionResult OnPost()
        {
            Vehiculo.Placa = Validaciones.FormatearPlaca(Vehiculo.Placa);
            Vehiculo.Marca = Validaciones.Capitalizar(Vehiculo.Marca);
            Vehiculo.Color = Validaciones.Capitalizar(Vehiculo.Color);

            if (!Validaciones.ValidarPlaca(Vehiculo.Placa))
            {
                ModelState.AddModelError(
                    "Vehiculo.Placa",
                    "La placa debe tener 3 o 4 números seguidos de 3 letras"
                );
            }

            if (!Validaciones.LetrasEspaciosGuion(Vehiculo.Marca))
            {
                ModelState.AddModelError(
                    "Vehiculo.Marca",
                    "La marca solo puede contener letras, espacios y guiones"
                );
            }

            if (!Validaciones.LetrasNumerosEspaciosGuion(Vehiculo.Modelo))
            {
                ModelState.AddModelError(
                    "Vehiculo.Modelo",
                    "El modelo solo puede contener letras, números, espacios y guiones"
                );
            }

            if (!Validaciones.SoloLetras(Vehiculo.Color))
            {
                ModelState.AddModelError(
                    "Vehiculo.Color",
                    "El color solo puede contener letras"
                );
            }

            if (!ModelState.IsValid)
            {
                CargarClientes();
                return Page();
            }

            Vehiculo.Estado = true;

            try
            {
                _connection.Open();

                string query = @"INSERT INTO vehiculo
                                 (placa, marca, modelo, anio, color, tipo, kilometraje, cliente_id, estado)
                                 VALUES
                                 (@placa, @marca, @modelo, @anio, @color, @tipo, @kilometraje, @cliente_id, @estado)";

                using MySqlCommand cmd = new MySqlCommand(query, _connection);

                cmd.Parameters.AddWithValue("@placa", Vehiculo.Placa);
                cmd.Parameters.AddWithValue("@marca", Vehiculo.Marca);
                cmd.Parameters.AddWithValue("@modelo", Vehiculo.Modelo);
                cmd.Parameters.AddWithValue("@anio", Vehiculo.Anio);
                cmd.Parameters.AddWithValue("@color", Vehiculo.Color);
                cmd.Parameters.AddWithValue("@tipo", Vehiculo.Tipo);
                cmd.Parameters.AddWithValue("@kilometraje", Vehiculo.Kilometraje);
                cmd.Parameters.AddWithValue("@cliente_id", Vehiculo.ClienteId);
                cmd.Parameters.AddWithValue("@estado", Vehiculo.Estado);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Error al guardar el vehículo: " + ex.Message
                );

                CargarClientes();
                return Page();
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return RedirectToPage("/Vehiculos");
        }

        private void CargarClientes()
        {
            ListaClientes.Clear();

            try
            {
                _connection.Open();

                string query = @"SELECT id, CONCAT(nombre, ' ', apellido) AS NombreCompleto
                                 FROM cliente
                                 WHERE estado = 1
                                 ORDER BY nombre ASC";

                using MySqlCommand cmd = new MySqlCommand(query, _connection);
                using MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ListaClientes.Add(new SelectListItem
                    {
                        Value = reader["id"].ToString(),
                        Text = reader["NombreCompleto"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar clientes: " + ex.Message);
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