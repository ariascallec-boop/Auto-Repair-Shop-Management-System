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

                using MySqlCommand cmd = new MySqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
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

        private void CargarVehiculos()
        {
            ListaVehiculos.Clear();

            try
            {
                _connection.Open();

                string query = @"SELECT v.id, v.placa, v.marca, v.modelo, v.anio, v.color, v.tipo, v.kilometraje, v.cliente_id, v.estado,
                                 CONCAT(c.nombre, ' ', c.apellido) AS cliente_nombre
                                 FROM vehiculo v
                                 INNER JOIN cliente c ON v.cliente_id = c.id
                                 WHERE v.estado = 1";

                if (!string.IsNullOrWhiteSpace(Buscar))
                {
                    query += @" AND (v.placa LIKE @buscar
                                OR v.marca LIKE @buscar
                                OR v.modelo LIKE @buscar
                                OR v.tipo LIKE @buscar
                                OR c.nombre LIKE @buscar
                                OR c.apellido LIKE @buscar
                                OR c.ci_nit LIKE @buscar)";
                }

                query += " ORDER BY v.id DESC";

                using MySqlCommand cmd = new MySqlCommand(query, _connection);

                if (!string.IsNullOrWhiteSpace(Buscar))
                {
                    cmd.Parameters.AddWithValue("@buscar", "%" + Buscar + "%");
                }

                using MySqlDataReader reader = cmd.ExecuteReader();

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
                        ClienteNombre = reader["cliente_nombre"]?.ToString() ?? "",
                        Estado = Convert.ToBoolean(reader["estado"])
                    });
                }
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