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

        // Token protegido que identifica al cliente.
        [BindProperty]
        public string Token { get; set; } = "";

        public IActionResult OnGet(string id)
        {
            int clienteId;

            try
            {
                // Validamos y recuperamos el ID real desde el token.
                clienteId = int.Parse(_protector.Unprotect(id));
            }
            catch
            {
                // Si el token fue modificado, es inválido
                // o no puede convertirse correctamente,
                // no se permite continuar.
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

                            // Conservamos el token para el POST.
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
            // Limpiamos espacios al principio y al final.
            Cliente.Nombre = Cliente.Nombre?.Trim();
            Cliente.Apellido = Cliente.Apellido?.Trim();
            Cliente.CiNit = Cliente.CiNit?.Trim();
            Cliente.Telefono = Cliente.Telefono?.Trim();
            Cliente.Email = Cliente.Email?.Trim();
            Cliente.Direccion = Cliente.Direccion?.Trim();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validación adicional del CI/NIT.
            if (!string.IsNullOrWhiteSpace(Cliente.CiNit) &&
                !Regex.IsMatch(Cliente.CiNit, "^[0-9]+$"))
            {
                ModelState.AddModelError(
                    "Cliente.CiNit",
                    "El CI/NIT debe contener solo dígitos");

                return Page();
            }

            int clienteId;

            try
            {
                // El ID utilizado para actualizar se obtiene
                // exclusivamente del token protegido.
                clienteId = int.Parse(_protector.Unprotect(Token));
            }
            catch
            {
                // Token inválido o manipulado.
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

                    // IMPORTANTE:
                    // No usamos Cliente.Id.
                    // Utilizamos el ID recuperado del token.
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
    }
}