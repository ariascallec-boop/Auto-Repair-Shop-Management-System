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

        // Lista para poblar el dropdown de clientes en la vista
        public List<SelectListItem> ListaClientes { get; set; } = new List<SelectListItem>();

        public void OnGet()
        {
            CargarClientes();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                CargarClientes(); // Recargar dropdown si falla la validación
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

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@placa", Vehiculo.Placa ?? "");
                    cmd.Parameters.AddWithValue("@marca", Vehiculo.Marca ?? "");
                    cmd.Parameters.AddWithValue("@modelo", Vehiculo.Modelo ?? "");
                    cmd.Parameters.AddWithValue("@anio", Vehiculo.Anio);
                    cmd.Parameters.AddWithValue("@color", Vehiculo.Color ?? "");
                    cmd.Parameters.AddWithValue("@tipo", Vehiculo.Tipo ?? "");
                    cmd.Parameters.AddWithValue("@kilometraje", Vehiculo.Kilometraje);
                    cmd.Parameters.AddWithValue("@cliente_id", Vehiculo.ClienteId);
                    cmd.Parameters.AddWithValue("@estado", Vehiculo.Estado);

                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                _connection.Close();
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

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListaClientes.Add(new SelectListItem
                            {
                                Value = reader["id"].ToString(),
                                Text = reader["NombreCompleto"].ToString()
                            });
                        }
                    }
                }
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}