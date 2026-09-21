using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public IActionResult OnGet(int id)
        {
            try
            {
                _connection.Open();

                string query = @"SELECT id,marca,modelo,anio,color,placa,estado,tipo,kilometraje,cliente_id
                                 FROM vehiculo
                                 WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Vehiculo.Id = Convert.ToInt32(reader["id"]);
                            Vehiculo.Marca = reader["marca"].ToString();
                            Vehiculo.Modelo = reader["modelo"].ToString();
                            Vehiculo.Anio = Convert.ToInt32(reader["anio"]);
                            Vehiculo.Color = reader["color"].ToString();
                            Vehiculo.Placa = reader["placa"].ToString();
                            Vehiculo.Estado = Convert.ToBoolean(reader["estado"]);
                            Vehiculo.Tipo = reader["tipo"].ToString();
                            Vehiculo.Kilometraje = Convert.ToInt32(reader["kilometraje"]);
                            Vehiculo.ClienteId = Convert.ToInt32(reader["cliente_id"]);
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
                return Page();
            }

            try
            {
                _connection.Open();

                string query = @"UPDATE vehiculo
                         SET marca = @marca,
                             modelo = @modelo,
                             anio = @anio,
                             color = @color,
                             placa = @placa,
                             tipo = @tipo,
                             kilometraje = @kilometraje,
                             cliente_id = @clienteId
                         WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@id", Vehiculo.Id);
                    cmd.Parameters.AddWithValue("@marca", Vehiculo.Marca ?? "");
                    cmd.Parameters.AddWithValue("@modelo", Vehiculo.Modelo ?? "");
                    cmd.Parameters.AddWithValue("@anio", Vehiculo.Anio);
                    cmd.Parameters.AddWithValue("@color", Vehiculo.Color ?? "");
                    cmd.Parameters.AddWithValue("@placa", Vehiculo.Placa ?? "");
                    cmd.Parameters.AddWithValue("@tipo", Vehiculo.Tipo ?? "");
                    cmd.Parameters.AddWithValue("@kilometraje", Vehiculo.Kilometraje);
                    cmd.Parameters.AddWithValue("@clienteId", Vehiculo.ClienteId);

                    int filas = cmd.ExecuteNonQuery();

                    if (filas > 0)
                    {
                        return RedirectToPage("/Vehiculos");
                    }

                    return Page();
                }
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", "Error de MySQL: " + ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return Page();
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}