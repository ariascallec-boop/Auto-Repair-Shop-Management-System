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

        [BindProperty]
        public string ClienteSeleccionado { get; set; } = "";

        public List<string> ListaClientes { get; set; } = new List<string>();

        public IActionResult OnGet(int id)
        {
            CargarClientes();

            try
            {
                _connection.Open();

                string query = @"SELECT v.id, v.placa, v.marca, v.modelo, v.anio, v.color, v.tipo, v.kilometraje, v.cliente_id, v.estado,
                                 c.nombre, c.apellido, c.ci_nit
                                 FROM vehiculo v
                                 INNER JOIN cliente c ON v.cliente_id = c.id
                                 WHERE v.id = @id";

                using MySqlCommand cmd = new MySqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@id", id);

                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Vehiculo = new Vehiculo
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
                    };

                    ClienteSeleccionado = reader["nombre"].ToString() + " " + reader["apellido"].ToString() + " - CI/NIT: " + reader["ci_nit"].ToString();
                }
                else
                {
                    return RedirectToPage("/Vehiculos");
                }
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            Vehiculo.Placa = Validaciones.FormatearPlaca(Vehiculo.Placa);
            Vehiculo.Marca = Validaciones.Capitalizar(Vehiculo.Marca);
            Vehiculo.Color = Validaciones.Capitalizar(Vehiculo.Color);

            if (!Validaciones.ValidarPlaca(Vehiculo.Placa))
            {
                ModelState.AddModelError("Vehiculo.Placa", "La placa debe tener 3 o 4 números seguidos de 3 letras");
            }

            if (!Validaciones.LetrasEspaciosGuion(Vehiculo.Marca))
            {
                ModelState.AddModelError("Vehiculo.Marca", "La marca solo puede contener letras, espacios y guiones");
            }

            if (!Validaciones.LetrasNumerosEspaciosGuion(Vehiculo.Modelo))
            {
                ModelState.AddModelError("Vehiculo.Modelo", "El modelo solo puede contener letras, números, espacios y guiones");
            }

            if (!Validaciones.SoloLetras(Vehiculo.Color))
            {
                ModelState.AddModelError("Vehiculo.Color", "El color solo puede contener letras");
            }

            int clienteId = BuscarClienteId(ClienteSeleccionado);

            ModelState.Remove("Vehiculo.ClienteId");

            if (clienteId == 0)
            {
                ModelState.AddModelError("ClienteSeleccionado", "Debe seleccionar un cliente válido");
            }
            else
            {
                Vehiculo.ClienteId = clienteId;
            }

            if (PlacaExisteEnOtroVehiculo(Vehiculo.Placa, Vehiculo.Id))
            {
                ModelState.AddModelError("Vehiculo.Placa", "La placa ya está registrada en otro vehículo");
            }

            if (!ModelState.IsValid)
            {
                CargarClientes();
                return Page();
            }

            try
            {
                _connection.Open();

                string query = @"UPDATE vehiculo
                                 SET placa = @placa, marca = @marca, modelo = @modelo, anio = @anio, color = @color,
                                 tipo = @tipo, kilometraje = @kilometraje, cliente_id = @cliente_id
                                 WHERE id = @id";

                using MySqlCommand cmd = new MySqlCommand(query, _connection);

                cmd.Parameters.AddWithValue("@placa", Vehiculo.Placa);
                cmd.Parameters.AddWithValue("@marca", Vehiculo.Marca);
                cmd.Parameters.AddWithValue("@modelo", Vehiculo.Modelo);
                cmd.Parameters.AddWithValue("@anio", Vehiculo.Anio);
                cmd.Parameters.AddWithValue("@color", Vehiculo.Color);
                cmd.Parameters.AddWithValue("@tipo", Vehiculo.Tipo);
                cmd.Parameters.AddWithValue("@kilometraje", Vehiculo.Kilometraje);
                cmd.Parameters.AddWithValue("@cliente_id", Vehiculo.ClienteId);
                cmd.Parameters.AddWithValue("@id", Vehiculo.Id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }

                ModelState.AddModelError("", "Error al actualizar el vehículo: " + ex.Message);
                CargarClientes();
                return Page();
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

        private void CargarClientes()
        {
            ListaClientes.Clear();

            try
            {
                _connection.Open();

                string query = @"SELECT id, nombre, apellido, ci_nit
                                 FROM cliente
                                 WHERE estado = 1
                                 ORDER BY nombre, apellido";

                using MySqlCommand cmd = new MySqlCommand(query, _connection);
                using MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string cliente = reader["nombre"].ToString() + " " + reader["apellido"].ToString() + " - CI/NIT: " + reader["ci_nit"].ToString();
                    ListaClientes.Add(cliente);
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

        private int BuscarClienteId(string clienteSeleccionado)
        {
            if (string.IsNullOrWhiteSpace(clienteSeleccionado))
            {
                return 0;
            }

            int clienteId = 0;

            try
            {
                _connection.Open();

                string query = @"SELECT id
                                 FROM cliente
                                 WHERE CONCAT(nombre, ' ', apellido, ' - CI/NIT: ', ci_nit) = @cliente
                                 AND estado = 1";

                using MySqlCommand cmd = new MySqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@cliente", clienteSeleccionado);

                object? resultado = cmd.ExecuteScalar();

                if (resultado != null)
                {
                    clienteId = Convert.ToInt32(resultado);
                }
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return clienteId;
        }

        private bool PlacaExisteEnOtroVehiculo(string placa, int id)
        {
            bool existe = false;

            try
            {
                _connection.Open();

                string query = @"SELECT COUNT(*)
                                 FROM vehiculo
                                 WHERE placa = @placa
                                 AND id <> @id";

                using MySqlCommand cmd = new MySqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@placa", placa);
                cmd.Parameters.AddWithValue("@id", id);

                int cantidad = Convert.ToInt32(cmd.ExecuteScalar());

                if (cantidad > 0)
                {
                    existe = true;
                }
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return existe;
        }
    }
}