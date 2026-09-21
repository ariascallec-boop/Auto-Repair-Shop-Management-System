using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public string Mensaje { get; set; } = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Mensaje = "Hay datos inválidos en el formulario.";
                return Page();
            }

            Vehiculo.Estado = true;

            try
            {
                _connection.Open();

                string query = @"INSERT INTO vehiculo
                                (marca, modelo, anio, color, placa, estado, tipo, kilometraje, cliente_id)
                                VALUES
                                (@marca, @modelo, @anio, @color, @placa, @estado, @tipo, @kilometraje, @clienteId)";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@marca", Vehiculo.Marca ?? "");
                    cmd.Parameters.AddWithValue("@modelo", Vehiculo.Modelo ?? "");
                    cmd.Parameters.AddWithValue("@anio", Vehiculo.Anio);
                    cmd.Parameters.AddWithValue("@color", Vehiculo.Color ?? "");
                    cmd.Parameters.AddWithValue("@placa", Vehiculo.Placa ?? "");
                    cmd.Parameters.AddWithValue("@estado", Vehiculo.Estado);
                    cmd.Parameters.AddWithValue("@tipo", Vehiculo.Tipo ?? "");
                    cmd.Parameters.AddWithValue("@kilometraje", Vehiculo.Kilometraje);
                    cmd.Parameters.AddWithValue("@clienteId", Vehiculo.ClienteId);

                    int filas = cmd.ExecuteNonQuery();

                    if (filas > 0)
                    {
                        return RedirectToPage("/Vehiculos");
                    }
                    else
                    {
                        Mensaje = "No se pudo registrar el vehículo.";
                        return Page();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Mensaje = "Error de MySQL: " + ex.Message;
                return Page();
            }
            catch (Exception ex)
            {
                Mensaje = "Error: " + ex.Message;
                return Page();
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}