using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class VehiculosModel : PageModel
    {
        private readonly MySqlConnection _connection;

        public VehiculosModel(MySqlConnection connection)
        {
            _connection = connection;
        }

        public List<Vehiculo> ListaVehiculos { get; set; } = new List<Vehiculo>();

        [BindProperty(SupportsGet = true)]
        public string Buscar { get; set; } = "";

        public void OnGet(string buscar)
        {
            Buscar = buscar ?? "";
            CargarVehiculos();
        }

        public IActionResult OnPostEliminar(int id)
        {
            try
            {
                _connection.Open();

                string query = "UPDATE vehiculo SET estado = 0 WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                _connection.Close();
            }

            return RedirectToPage("/Vehiculos");
        }

        private void CargarVehiculos()
        {
            ListaVehiculos.Clear();

            try
            {
                _connection.Open();

                string query = @"SELECT id, placa, marca, modelo, anio, color, tipo, kilometraje, cliente_id, estado 
                                 FROM vehiculo 
                                 WHERE estado = 1";

                if (!string.IsNullOrWhiteSpace(Buscar))
                {
                    query += @" AND (placa LIKE @buscar 
                                 OR marca LIKE @buscar 
                                 OR modelo LIKE @buscar 
                                 OR tipo LIKE @buscar)";
                }

                query += " ORDER BY id DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    if (!string.IsNullOrWhiteSpace(Buscar))
                    {
                        cmd.Parameters.AddWithValue("@buscar", $"%{Buscar}%");
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListaVehiculos.Add(new Vehiculo
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