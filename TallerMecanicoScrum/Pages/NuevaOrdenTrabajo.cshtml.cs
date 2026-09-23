using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class NuevaOrdenTrabajoModel : PageModel
    {
        private readonly MySqlConnection _connection;

        public NuevaOrdenTrabajoModel(MySqlConnection connection)
        {
            _connection = connection;
        }

        [BindProperty]
        public OrdenTrabajo EntidadOrden { get; set; } = new OrdenTrabajo();

        public List<VehiculoDisponible> ListaVehiculos { get; set; } = new List<VehiculoDisponible>();


        public void OnGet()
        {
            CargarVehiculos();

            if (EntidadOrden.FechaEntregaEstimada == DateTime.MinValue)
            {
                EntidadOrden.FechaEntregaEstimada = DateTime.Now.AddDays(1);
            }
        }


        private void CargarVehiculos()
        {
            try
            {
                string query = @"
                    SELECT
                        v.id,
                        v.placa,
                        v.marca,
                        v.modelo,
                        CONCAT(c.nombre, ' ', c.apellido) AS cliente
                    FROM vehiculo v
                    INNER JOIN cliente c
                        ON v.cliente_id = c.id
                    WHERE v.estado = TRUE
                    AND c.estado = TRUE
                    ORDER BY v.id DESC;";

                _connection.Open();

                using MySqlCommand command = new MySqlCommand(query, _connection);
                using MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    VehiculoDisponible vehiculo = new VehiculoDisponible();

                    vehiculo.Id = Convert.ToInt32(reader["id"]);
                    vehiculo.Placa = reader["placa"].ToString() ?? "";
                    vehiculo.Marca = reader["marca"].ToString() ?? "";
                    vehiculo.Modelo = reader["modelo"].ToString() ?? "";
                    vehiculo.Cliente = reader["cliente"].ToString() ?? "";

                    ListaVehiculos.Add(vehiculo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar vehículos: " + ex.Message);
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }


        public IActionResult OnPost()
        {
            try
            {
                if (EntidadOrden.VehiculoId == 0)
                {
                    ModelState.AddModelError(
                        "EntidadOrden.VehiculoId",
                        "Debe seleccionar un vehículo."
                    );

                    CargarVehiculos();

                    return Page();
                }


                if (EntidadOrden.Diagnostico == "")
                {
                    ModelState.AddModelError(
                        "EntidadOrden.Diagnostico",
                        "Debe ingresar un diagnóstico."
                    );

                    CargarVehiculos();

                    return Page();
                }


                string query = @"
                    INSERT INTO orden_trabajo
                    (
                        vehiculo_id,
                        fecha_entrega_estimada,
                        estado,
                        diagnostico,
                        activo
                    )
                    VALUES
                    (
                        @vehiculoId,
                        @fechaEntrega,
                        'recibido',
                        @diagnostico,
                        TRUE
                    );";


                _connection.Open();

                using MySqlCommand command = new MySqlCommand(query, _connection);

                command.Parameters.AddWithValue(
                    "@vehiculoId",
                    EntidadOrden.VehiculoId
                );

                command.Parameters.AddWithValue(
                    "@fechaEntrega",
                    EntidadOrden.FechaEntregaEstimada
                );

                command.Parameters.AddWithValue(
                    "@diagnostico",
                    EntidadOrden.Diagnostico
                );

                command.ExecuteNonQuery();

                return RedirectToPage("/OrdenesTrabajo");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al crear orden: " + ex.Message);

                CargarVehiculos();

                return Page();
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


    public class VehiculoDisponible
    {
        public int Id { get; set; }

        public string Placa { get; set; } = "";

        public string Marca { get; set; } = "";

        public string Modelo { get; set; } = "";

        public string Cliente { get; set; } = "";
    }
}