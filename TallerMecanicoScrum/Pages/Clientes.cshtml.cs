using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class ClientesModel : PageModel
    {
        private readonly MySqlConnection _connection;
        private readonly IDataProtector _protector;

        public ClientesModel(
            MySqlConnection connection,
            IDataProtectionProvider dataProtectionProvider)
        {
            _connection = connection;

            _protector = dataProtectionProvider.CreateProtector(
                "TallerMecanicoScrum.ClienteId");
        }

        public List<Cliente> ListaClientes { get; set; } = new List<Cliente>();

        [BindProperty(SupportsGet = true)]
        public string Buscar { get; set; } = "";

        public void OnGet(string buscar)
        {
            Buscar = buscar ?? "";

            if (!string.IsNullOrWhiteSpace(Buscar))
            {
                BuscarClientes();
            }
            else
            {
                CargarClientes();
            }
        }

        // Genera un token protegido para utilizarlo en la URL
        // y evitar exponer directamente el ID real del cliente.
        public string GenerarToken(int id)
        {
            return _protector.Protect(id.ToString());
        }

        public IActionResult OnPostEliminar(string token)
        {
            int id;

            try
            {
                // Recuperamos el ID real a partir del token protegido.
                id = int.Parse(_protector.Unprotect(token));
            }
            catch
            {
                // Si el token fue alterado o no es válido,
                // no se realiza ninguna operación.
                return RedirectToPage("/Clientes");
            }

            try
            {
                _connection.Open();

                // Borrado lógico del cliente.
                string query = @"UPDATE cliente
                                 SET estado = 0
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

            return RedirectToPage("/Clientes");
        }

        private void CargarClientes()
        {
            ListaClientes.Clear();

            try
            {
                _connection.Open();

                string query = @"SELECT id, ci_nit, nombre, apellido, telefono, email, direccion, estado
                                 FROM cliente
                                 WHERE estado = 1";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListaClientes.Add(new Cliente
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                CiNit = reader["ci_nit"]?.ToString() ?? "",
                                Nombre = reader["nombre"]?.ToString() ?? "",
                                Apellido = reader["apellido"]?.ToString() ?? "",
                                Telefono = reader["telefono"]?.ToString() ?? "",
                                Email = reader["email"]?.ToString() ?? "",
                                Direccion = reader["direccion"]?.ToString() ?? "",
                                Estado = Convert.ToBoolean(reader["estado"])
                            });
                        }
                    }
                }
            }
            finally
            {
                _connection.Close();
            }
        }

        private void BuscarClientes()
        {
            ListaClientes.Clear();

            try
            {
                _connection.Open();

                string query = @"SELECT id, ci_nit, nombre, apellido, telefono, email, direccion, estado
                                 FROM cliente
                                 WHERE estado = 1
                                 AND (
                                     nombre LIKE @buscar
                                     OR apellido LIKE @buscar
                                     OR ci_nit LIKE @buscar
                                     OR email LIKE @buscar
                                 )";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@buscar", "%" + Buscar + "%");

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListaClientes.Add(new Cliente
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                CiNit = reader["ci_nit"]?.ToString() ?? "",
                                Nombre = reader["nombre"]?.ToString() ?? "",
                                Apellido = reader["apellido"]?.ToString() ?? "",
                                Telefono = reader["telefono"]?.ToString() ?? "",
                                Email = reader["email"]?.ToString() ?? "",
                                Direccion = reader["direccion"]?.ToString() ?? "",
                                Estado = Convert.ToBoolean(reader["estado"])
                            });
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