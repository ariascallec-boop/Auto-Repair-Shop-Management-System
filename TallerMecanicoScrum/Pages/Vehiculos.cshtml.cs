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

                string query = @"UPDATE vehiculo
                                 SET estado = false
                                 WHERE id = @id";

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
            try
            {
                _connection.Open();

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
                }

                query += " ORDER BY id DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    if (!string.IsNullOrWhiteSpace(Buscar))
                    {
                        cmd.Parameters.AddWithValue("@buscar", "%" + Buscar + "%");
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
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