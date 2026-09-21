using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class EditarVehiculoModel : PageModel
    {
        private readonly MySqlConnection _connection;

        public EditarVehiculoModel(MySqlConnection connection)
        {
            _connection = connection;
        }

        [BindProperty]
        public Vehiculo Vehiculo { get; set; } = new Vehiculo();

        // Lista para popular el desplegable de clientes en la vista
        public List<SelectListItem> ListaClientes { get; set; } = new List<SelectListItem>();

        public IActionResult OnGet(int id)
        {
            CargarClientes();

            try
            {
                _connection.Open();

                string query = @"SELECT id, placa, marca, modelo, anio, color, tipo, kilometraje, cliente_id, estado 
                                 FROM vehiculo 
                                 WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Vehiculo = new Vehiculo
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Placa = reader["placa"]?.ToString() ?? "",
                                Marca = reader["marca"]?.ToString() ?? "",
                                Modelo = reader["modelo"]?.ToString() ?? "",
                                Anio = Convert.ToInt32(reader["anio"]),
                                Color = reader["color"]?.ToString() ?? "",
                                Tipo = reader["tipo"]?.ToString() ?? "",
                                Kilometraje = Convert.ToInt32(reader["kilometraje"]),
                                ClienteId = Convert.ToInt32(reader["cliente_id"]),
                                Estado = Convert.ToBoolean(reader["estado"])
                            };
                        }
                        else
                        {
                            return RedirectToPage("/Vehiculos");
                        }
                    }
                }
            }
            finally
            {
                _connection.Close();
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                CargarClientes(); // Volver a cargar el selector si falla la validación
                return Page();
            }

            try
            {
                _connection.Open();

                string query = @"UPDATE vehiculo 
                                 SET placa = @placa, 
                                     marca = @marca, 
                                     modelo = @modelo, 
                                     anio = @anio, 
                                     color = @color, 
                                     tipo = @tipo, 
                                     kilometraje = @kilometraje, 
                                     cliente_id = @cliente_id 
                                 WHERE id = @id";

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
                    cmd.Parameters.AddWithValue("@id", Vehiculo.Id);

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