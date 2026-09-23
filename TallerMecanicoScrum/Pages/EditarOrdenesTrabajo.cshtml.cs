using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class EditarOrdenesTrabajoModel : PageModel
    {
        private readonly MySqlConnection _connection;

        public EditarOrdenesTrabajoModel(MySqlConnection connection)
        {
            _connection = connection;
        }


        [BindProperty]
        public OrdenTrabajo EntidadOrden { get; set; } = new OrdenTrabajo();


        public string Placa { get; set; } = "";

        public string Cliente { get; set; } = "";

        public string Vehiculo { get; set; } = "";


        public void OnGet(int id)
        {
            CargarOrden(id);
        }


        private void CargarOrden(int id)
        {
            try
            {
                string query = @"
                    SELECT
                        ot.id,
                        ot.vehiculo_id,
                        ot.fecha_ingreso,
                        ot.fecha_entrega_estimada,
                        ot.estado,
                        ot.diagnostico,
                        ot.fecha_notificacion,
                        ot.observaciones_notificacion,
                        v.placa,
                        CONCAT(v.marca, ' ', v.modelo) AS vehiculo,
                        CONCAT(c.nombre, ' ', c.apellido) AS cliente
                    FROM orden_trabajo ot
                    INNER JOIN vehiculo v
                        ON ot.vehiculo_id = v.id
                    INNER JOIN cliente c
                        ON v.cliente_id = c.id
                    WHERE ot.id = @id
                    AND ot.activo = TRUE;";


                _connection.Open();

                using MySqlCommand command = new MySqlCommand(query, _connection);

                command.Parameters.AddWithValue("@id", id);

                using MySqlDataReader reader = command.ExecuteReader();


                if (reader.Read())
                {
                    EntidadOrden.Id =
                        Convert.ToInt32(reader["id"]);

                    EntidadOrden.VehiculoId =
                        Convert.ToInt32(reader["vehiculo_id"]);

                    EntidadOrden.FechaIngreso =
                        Convert.ToDateTime(reader["fecha_ingreso"]);

                    EntidadOrden.FechaEntregaEstimada =
                        Convert.ToDateTime(reader["fecha_entrega_estimada"]);

                    EntidadOrden.Estado =
                        reader["estado"].ToString() ?? "";

                    EntidadOrden.Diagnostico =
                        reader["diagnostico"].ToString() ?? "";


                    if (reader["fecha_notificacion"] != DBNull.Value)
                    {
                        EntidadOrden.FechaNotificacion =
                            Convert.ToDateTime(reader["fecha_notificacion"]);
                    }


                    if (reader["observaciones_notificacion"] != DBNull.Value)
                    {
                        EntidadOrden.ObservacionesNotificacion =
                            reader["observaciones_notificacion"].ToString() ?? "";
                    }


                    Placa =
                        reader["placa"].ToString() ?? "";

                    Cliente =
                        reader["cliente"].ToString() ?? "";

                    Vehiculo =
                        reader["vehiculo"].ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Error al cargar orden: " + ex.Message
                );
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
                string query = @"
                    UPDATE orden_trabajo
                    SET
                        fecha_entrega_estimada = @fechaEntrega,
                        estado = @estado,
                        diagnostico = @diagnostico,
                        fecha_notificacion = @fechaNotificacion,
                        observaciones_notificacion = @observacionesNotificacion
                    WHERE id = @id
                    AND activo = TRUE;";


                _connection.Open();

                using MySqlCommand command =
                    new MySqlCommand(query, _connection);


                command.Parameters.AddWithValue(
                    "@fechaEntrega",
                    EntidadOrden.FechaEntregaEstimada
                );


                command.Parameters.AddWithValue(
                    "@estado",
                    EntidadOrden.Estado
                );


                command.Parameters.AddWithValue(
                    "@diagnostico",
                    EntidadOrden.Diagnostico
                );


                if (EntidadOrden.FechaNotificacion.HasValue)
                {
                    command.Parameters.AddWithValue(
                        "@fechaNotificacion",
                        EntidadOrden.FechaNotificacion.Value
                    );
                }
                else
                {
                    command.Parameters.AddWithValue(
                        "@fechaNotificacion",
                        DBNull.Value
                    );
                }


                if (EntidadOrden.ObservacionesNotificacion == "")
                {
                    command.Parameters.AddWithValue(
                        "@observacionesNotificacion",
                        DBNull.Value
                    );
                }
                else
                {
                    command.Parameters.AddWithValue(
                        "@observacionesNotificacion",
                        EntidadOrden.ObservacionesNotificacion
                    );
                }


                command.Parameters.AddWithValue(
                    "@id",
                    EntidadOrden.Id
                );


                command.ExecuteNonQuery();


                return RedirectToPage("/OrdenesTrabajo");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Error al actualizar orden: " + ex.Message
                );

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
}