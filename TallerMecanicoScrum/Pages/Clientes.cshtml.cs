using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class ClientesModel : PageModel
    {
        private readonly MySqlConnection _connection;

        public ClientesModel(MySqlConnection connection)
        {
            _connection = connection;
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

        public IActionResult OnPostEliminar(int id)
        {
            try
            {
                _connection.Open();

                // Borrado lógico usando 'id' en lugar de 'id_cliente'
                string query = "UPDATE cliente SET estado = 0 WHERE id = @id";

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

                // Seleccionamos 'id' ajustado a tu esquema real
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
                                 WHERE estado = 1 AND (
                                     nombre LIKE @buscar OR 
                                     apellido LIKE @buscar OR 
                                     ci_nit LIKE @buscar OR 
                                     email LIKE @buscar
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