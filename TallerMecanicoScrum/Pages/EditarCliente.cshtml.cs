using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoScrum.Clases;
using System.Text.RegularExpressions;

namespace TallerMecanicoScrum.Pages
{
    public class EditarClienteModel : PageModel
    {
        private readonly MySqlConnection _connection;
        private readonly IDataProtector _protector;

        public EditarClienteModel(
            MySqlConnection connection,
            IDataProtectionProvider dataProtectionProvider)
        {
            _connection = connection;

            _protector = dataProtectionProvider.CreateProtector(
                "TallerMecanicoScrum.ClienteId");
        }

        [BindProperty]
        public Cliente Cliente { get; set; } = new Cliente();

        [BindProperty]
        public string Token { get; set; } = "";

        public IActionResult OnGet(string id)
        {
            int clienteId;

            try
            {
                clienteId = int.Parse(_protector.Unprotect(id));
            }
            catch
            {
                return RedirectToPage("/Clientes");
            }

            try
            {
                _connection.Open();

                string query = @"SELECT id, ci_nit, nombre, apellido, telefono, email, direccion, estado
                                 FROM cliente
                                 WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@id", clienteId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Cliente = new Cliente
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                CiNit = reader["ci_nit"]?.ToString() ?? "",
                                Nombre = reader["nombre"]?.ToString() ?? "",
                                Apellido = reader["apellido"]?.ToString() ?? "",
                                Telefono = reader["telefono"]?.ToString() ?? "",
                                Email = reader["email"]?.ToString() ?? "",
                                Direccion = reader["direccion"]?.ToString() ?? "",
                                Estado = Convert.ToBoolean(reader["estado"])
                            };

                            Token = id;
                        }
                        else
                        {
                            return RedirectToPage("/Clientes");
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
            Cliente.Nombre = NormalizarEspacios(Cliente.Nombre);
            Cliente.Apellido = NormalizarEspacios(Cliente.Apellido);
            Cliente.CiNit = NormalizarEspacios(Cliente.CiNit);
            Cliente.Telefono = NormalizarEspacios(Cliente.Telefono);
            Cliente.Email = NormalizarEspacios(Cliente.Email);
            Cliente.Direccion = NormalizarEspacios(Cliente.Direccion);

            Cliente.Nombre = NormalizarNombre(Cliente.Nombre);
            Cliente.Apellido = NormalizarNombre(Cliente.Apellido);

            ModelState.Clear();
            TryValidateModel(Cliente, nameof(Cliente));

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!string.IsNullOrWhiteSpace(Cliente.CiNit) &&
                !Regex.IsMatch(Cliente.CiNit, "^[0-9]+$"))
            {
                ModelState.AddModelError(
                    "Cliente.CiNit",
                    "El CI/NIT debe contener solo dígitos.");

                return Page();
            }

            int clienteId;

            try
            {
                clienteId = int.Parse(_protector.Unprotect(Token));
            }
            catch
            {
                return RedirectToPage("/Clientes");
            }

            try
            {
                _connection.Open();

                string query = @"UPDATE cliente
                                 SET nombre = @nombre,
                                     apellido = @apellido,
                                     ci_nit = @ci_nit,
                                     telefono = @telefono,
                                     email = @email,
                                     direccion = @direccion
                                 WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@nombre", Cliente.Nombre ?? "");
                    cmd.Parameters.AddWithValue("@apellido", Cliente.Apellido ?? "");
                    cmd.Parameters.AddWithValue("@ci_nit", Cliente.CiNit ?? "");
                    cmd.Parameters.AddWithValue("@telefono", Cliente.Telefono ?? "");
                    cmd.Parameters.AddWithValue("@email", Cliente.Email ?? "");
                    cmd.Parameters.AddWithValue("@direccion", Cliente.Direccion ?? "");
                    cmd.Parameters.AddWithValue("@id", clienteId);

                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                _connection.Close();
            }

            return RedirectToPage("/Clientes");
        }

        private string NormalizarEspacios(string texto)
        {
            if (texto == null)
            {
                return "";
            }

            texto = texto.Trim();
            texto = Regex.Replace(texto, "\\s+", " ");

            return texto;
        }

        private string NormalizarNombre(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return texto;
            }

            string[] partes = texto.Split(' ');

            for (int i = 0; i < partes.Length; i++)
            {
                string parte = partes[i].ToLower();

                if (parte.Length > 0)
                {
                    partes[i] = char.ToUpper(parte[0]) + parte.Substring(1);
                }
            }

            return string.Join(" ", partes);
        }
    }
}