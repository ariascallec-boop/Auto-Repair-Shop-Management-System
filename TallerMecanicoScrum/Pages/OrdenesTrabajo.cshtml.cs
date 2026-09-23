using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace TallerMecanicoScrum.Pages
{
    public class OrdenesTrabajoModel : PageModel
    {
        private readonly MySqlConnection _connection;

        public OrdenesTrabajoModel(MySqlConnection connection)
        {
            _connection = connection;
        }

        public List<OrdenTrabajoResumen> ListaOrdenes { get; set; } = new List<OrdenTrabajoResumen>();

        public string Buscar { get; set; } = "";

        public void OnGet(string buscar)
        {
            Buscar = buscar ?? "";

            if (Buscar == "")
            {
                CargarOrdenes();
            }
            else
            {
                BuscarOrdenes();
            }
        }

        private void CargarOrdenes()
        {
            try
            {
                string query = @"
                    SELECT
                        ot.id,
                        ot.vehiculo_id,
                        v.placa,
                        CONCAT(c.nombre, ' ', c.apellido) AS cliente,
                        CONCAT(v.marca, ' ', v.modelo) AS vehiculo,
                        ot.fecha_ingreso,
                        ot.estado,
                        ot.fecha_entrega_estimada,
                        CONCAT(m.nombre, ' ', m.apellido) AS responsable
                    FROM orden_trabajo ot
                    INNER JOIN vehiculo v
                        ON ot.vehiculo_id = v.id
                    INNER JOIN cliente c
                        ON v.cliente_id = c.id
                    LEFT JOIN orden_mecanico om
                        ON ot.id = om.orden_id
                        AND om.es_responsable = TRUE
                    LEFT JOIN mecanico m
                        ON om.mecanico_id = m.id
                    WHERE ot.activo = TRUE
                    AND v.estado = TRUE
                    AND c.estado = TRUE
                    ORDER BY ot.id DESC;";

                _connection.Open();

                using MySqlCommand command = new MySqlCommand(query, _connection);
                using MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    OrdenTrabajoResumen orden = new OrdenTrabajoResumen();

                    orden.Id = Convert.ToInt32(reader["id"]);
                    orden.VehiculoId = Convert.ToInt32(reader["vehiculo_id"]);
                    orden.Placa = reader["placa"].ToString() ?? "";
                    orden.Cliente = reader["cliente"].ToString() ?? "";
                    orden.Vehiculo = reader["vehiculo"].ToString() ?? "";
                    orden.FechaIngreso = Convert.ToDateTime(reader["fecha_ingreso"]);
                    orden.Estado = reader["estado"].ToString() ?? "";
                    orden.FechaEntregaEstimada = Convert.ToDateTime(reader["fecha_entrega_estimada"]);

                    if (reader["responsable"] == DBNull.Value)
                    {
                        orden.Responsable = "Sin asignar";
                    }
                    else
                    {
                        orden.Responsable = reader["responsable"].ToString() ?? "Sin asignar";
                    }

                    ListaOrdenes.Add(orden);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar órdenes: " + ex.Message);
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        private void BuscarOrdenes()
        {
            try
            {
                string query = @"
                    SELECT
                        ot.id,
                        ot.vehiculo_id,
                        v.placa,
                        CONCAT(c.nombre, ' ', c.apellido) AS cliente,
                        CONCAT(v.marca, ' ', v.modelo) AS vehiculo,
                        ot.fecha_ingreso,
                        ot.estado,
                        ot.fecha_entrega_estimada,
                        CONCAT(m.nombre, ' ', m.apellido) AS responsable
                    FROM orden_trabajo ot
                    INNER JOIN vehiculo v
                        ON ot.vehiculo_id = v.id
                    INNER JOIN cliente c
                        ON v.cliente_id = c.id
                    LEFT JOIN orden_mecanico om
                        ON ot.id = om.orden_id
                        AND om.es_responsable = TRUE
                    LEFT JOIN mecanico m
                        ON om.mecanico_id = m.id
                    WHERE ot.activo = TRUE
                    AND v.estado = TRUE
                    AND c.estado = TRUE
                    AND (
                        v.placa LIKE @buscar
                        OR c.nombre LIKE @buscar
                        OR c.apellido LIKE @buscar
                        OR v.marca LIKE @buscar
                        OR v.modelo LIKE @buscar
                        OR ot.estado LIKE @buscar
                    )
                    ORDER BY ot.id DESC;";

                _connection.Open();

                using MySqlCommand command = new MySqlCommand(query, _connection);

                command.Parameters.AddWithValue("@buscar", "%" + Buscar + "%");

                using MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    OrdenTrabajoResumen orden = new OrdenTrabajoResumen();

                    orden.Id = Convert.ToInt32(reader["id"]);
                    orden.VehiculoId = Convert.ToInt32(reader["vehiculo_id"]);
                    orden.Placa = reader["placa"].ToString() ?? "";
                    orden.Cliente = reader["cliente"].ToString() ?? "";
                    orden.Vehiculo = reader["vehiculo"].ToString() ?? "";
                    orden.FechaIngreso = Convert.ToDateTime(reader["fecha_ingreso"]);
                    orden.Estado = reader["estado"].ToString() ?? "";
                    orden.FechaEntregaEstimada = Convert.ToDateTime(reader["fecha_entrega_estimada"]);

                    if (reader["responsable"] == DBNull.Value)
                    {
                        orden.Responsable = "Sin asignar";
                    }
                    else
                    {
                        orden.Responsable = reader["responsable"].ToString() ?? "Sin asignar";
                    }

                    ListaOrdenes.Add(orden);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al buscar órdenes: " + ex.Message);
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        public IActionResult OnPostEliminar(int id)
        {
            try
            {
                string query = @"
                    UPDATE orden_trabajo
                    SET activo = FALSE
                    WHERE id = @id;";

                _connection.Open();

                using MySqlCommand command = new MySqlCommand(query, _connection);

                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar orden: " + ex.Message);
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return RedirectToPage("/OrdenesTrabajo");
        }
    }

    public class OrdenTrabajoResumen
    {
        public int Id { get; set; }

        public int VehiculoId { get; set; }

        public string Placa { get; set; } = "";

        public string Cliente { get; set; } = "";

        public string Vehiculo { get; set; } = "";

        public DateTime FechaIngreso { get; set; }

        public string Estado { get; set; } = "";

        public DateTime FechaEntregaEstimada { get; set; }

        public string Responsable { get; set; } = "";
    }
}