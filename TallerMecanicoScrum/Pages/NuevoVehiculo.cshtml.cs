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

        [BindProperty]
        public string ClienteSeleccionado { get; set; } = "";

        public List<string> ListaClientes { get; set; } = new List<string>();

        public void OnGet()
        {
            CargarClientes();
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

            if (PlacaExiste(Vehiculo.Placa))
            {
                ModelState.AddModelError("Vehiculo.Placa", "La placa ya se encuentra registrada");
            }

            if (!ModelState.IsValid)
            {
                CargarClientes();
                return Page();
            }

            Vehiculo.Estado = true;

            try
            {
                _connection.Open();

                string query = @"INSERT INTO vehiculo
                                 (placa, marca, modelo, anio, color, tipo, kilometraje, cliente_id, estado)
                                 VALUES
                                 (@placa, @marca, @modelo, @anio, @color, @tipo, @kilometraje, @cliente_id, @estado)";

                using MySqlCommand cmd = new MySqlCommand(query, _connection);

                cmd.Parameters.AddWithValue("@placa", Vehiculo.Placa);
                cmd.Parameters.AddWithValue("@marca", Vehiculo.Marca);
                cmd.Parameters.AddWithValue("@modelo", Vehiculo.Modelo);
                cmd.Parameters.AddWithValue("@anio", Vehiculo.Anio);
                cmd.Parameters.AddWithValue("@color", Vehiculo.Color);
                cmd.Parameters.AddWithValue("@tipo", Vehiculo.Tipo);
                cmd.Parameters.AddWithValue("@kilometraje", Vehiculo.Kilometraje);
                cmd.Parameters.AddWithValue("@cliente_id", Vehiculo.ClienteId);
                cmd.Parameters.AddWithValue("@estado", Vehiculo.Estado);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }

                ModelState.AddModelError("", "Error al guardar el vehículo: " + ex.Message);
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

        private bool PlacaExiste(string placa)
        {
            bool existe = false;

            try
            {
                _connection.Open();

                string query = @"SELECT COUNT(*)
                                 FROM vehiculo
                                 WHERE placa = @placa";

                using MySqlCommand cmd = new MySqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@placa", placa);

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