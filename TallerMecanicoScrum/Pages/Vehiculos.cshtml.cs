using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class VehiculosModel : PageModel
    {
        private readonly MySqlConnection _connection;
        private readonly IDataProtector _protector;

        public VehiculosModel(
            MySqlConnection connection,
            IDataProtectionProvider dataProtectionProvider)
        {
            _connection = connection;

            _protector = dataProtectionProvider.CreateProtector(
                "TallerMecanicoScrum.VehiculoId");
        }

        public List<Vehiculo> ListaVehiculos { get; set; } = new List<Vehiculo>();

        [BindProperty(SupportsGet = true)]
        public string Buscar { get; set; } = "";

        public void OnGet(string buscar)
        {
            Buscar = buscar ?? "";

            CargarVehiculos();
        }

        // Genera un token protegido a partir del ID del vehículo
        public string GenerarToken(int id)
        {
            return _protector.Protect(id.ToString());
        }

        public IActionResult OnPostEliminar(string token)
        {
            int vehiculoId;

            try
            {
                vehiculoId = int.Parse(_protector.Unprotect(token));
            }
            catch
            {
                return RedirectToPage("/Vehiculos");
            }

            try
            {
                _connection.Open();

                string query = @"UPDATE vehiculo
                                 SET estado = 0
                                 WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@id", vehiculoId);

                    cmd.ExecuteNonQuery();
                }
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

                string query = @"SELECT 
                                    v.id,
                                    v.placa,
                                    v.marca,
                                    v.modelo,
                                    v.anio,
                                    v.color,
                                    v.tipo,
                                    v.kilometraje,
                                    v.cliente_id,
                                    v.estado,
                                    CONCAT(c.nombre, ' ', c.apellido) AS cliente_nombre
                                 FROM vehiculo v
                                 INNER JOIN cliente c ON v.cliente_id = c.id
                                 WHERE v.estado = 1";

                if (!string.IsNullOrWhiteSpace(Buscar))
                {
                    query += @" AND (
                                    v.placa LIKE @buscar
                                    OR v.marca LIKE @buscar
                                    OR v.modelo LIKE @buscar
                                    OR v.tipo LIKE @buscar
                                    OR c.nombre LIKE @buscar
                                    OR c.apellido LIKE @buscar
                                    OR c.ci_nit LIKE @buscar
                                )";
                }

                query += " ORDER BY v.id DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    if (!string.IsNullOrWhiteSpace(Buscar))
                    {
                        cmd.Parameters.AddWithValue(
                            "@buscar",
                            "%" + Buscar + "%");
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Vehiculo vehiculo = new Vehiculo();

                            vehiculo.Id = Convert.ToInt32(reader["id"]);
                            vehiculo.Placa = reader["placa"].ToString();
                            vehiculo.Marca = reader["marca"].ToString();
                            vehiculo.Modelo = reader["modelo"].ToString();
                            vehiculo.Anio = Convert.ToInt32(reader["anio"]);
                            vehiculo.Color = reader["color"].ToString();
                            vehiculo.Tipo = reader["tipo"].ToString();
                            vehiculo.Kilometraje =
                                Convert.ToInt32(reader["kilometraje"]);
                            vehiculo.ClienteId =
                                Convert.ToInt32(reader["cliente_id"]);
                            vehiculo.ClienteNombre =
                                reader["cliente_nombre"].ToString();
                            vehiculo.Estado =
                                Convert.ToBoolean(reader["estado"]);

                            ListaVehiculos.Add(vehiculo);
                        }
                    }
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