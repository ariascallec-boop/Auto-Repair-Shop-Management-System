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
<<<<<<< HEAD

=======
>>>>>>> origin/feature/frontend
            CargarVehiculos();
        }

        public IActionResult OnPostEliminar(int id)
        {
            try
            {
                _connection.Open();

<<<<<<< HEAD
                string query = @"UPDATE vehiculo
                                 SET estado = false
                                 WHERE id = @id";
=======
                string query = "UPDATE vehiculo SET estado = 0 WHERE id = @id";
>>>>>>> origin/feature/frontend

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
<<<<<<< HEAD

=======
>>>>>>> origin/feature/frontend
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
<<<<<<< HEAD
=======
            ListaVehiculos.Clear();

>>>>>>> origin/feature/frontend
            try
            {
                _connection.Open();

<<<<<<< HEAD
                string query = @"SELECT
                                    id,
                                    marca,
                                    modelo,
                                    anio,
                                    color,
                                    placa,
                                    estado,
                                    tipo,
                                    kilometraje,
                                    cliente_id
                                 FROM vehiculo";

                if (!string.IsNullOrWhiteSpace(Buscar))
                {
                    query += @" WHERE placa LIKE @buscar
                                OR marca LIKE @buscar
                                OR modelo LIKE @buscar
                                OR tipo LIKE @buscar";
=======
                string query = @"SELECT id, placa, marca, modelo, anio, color, tipo, kilometraje, cliente_id, estado 
                                 FROM vehiculo 
                                 WHERE estado = 1";

                if (!string.IsNullOrWhiteSpace(Buscar))
                {
                    query += @" AND (placa LIKE @buscar 
                                 OR marca LIKE @buscar 
                                 OR modelo LIKE @buscar 
                                 OR tipo LIKE @buscar)";
>>>>>>> origin/feature/frontend
                }

                query += " ORDER BY id DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    if (!string.IsNullOrWhiteSpace(Buscar))
                    {
<<<<<<< HEAD
                        cmd.Parameters.AddWithValue("@buscar", "%" + Buscar + "%");
=======
                        cmd.Parameters.AddWithValue("@buscar", $"%{Buscar}%");
>>>>>>> origin/feature/frontend
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
<<<<<<< HEAD
                            Vehiculo vehiculo = new Vehiculo();

                            vehiculo.Id = Convert.ToInt32(reader["id"]);
                            vehiculo.Marca = reader["marca"].ToString();
                            vehiculo.Modelo = reader["modelo"].ToString();
                            vehiculo.Anio = Convert.ToInt32(reader["anio"]);
                            vehiculo.Color = reader["color"].ToString();
                            vehiculo.Placa = reader["placa"].ToString();
                            vehiculo.Estado = Convert.ToBoolean(reader["estado"]);
                            vehiculo.Tipo = reader["tipo"].ToString();
                            vehiculo.Kilometraje = Convert.ToInt32(reader["kilometraje"]);
                            vehiculo.ClienteId = Convert.ToInt32(reader["cliente_id"]);

                            ListaVehiculos.Add(vehiculo);
=======
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
>>>>>>> origin/feature/frontend
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