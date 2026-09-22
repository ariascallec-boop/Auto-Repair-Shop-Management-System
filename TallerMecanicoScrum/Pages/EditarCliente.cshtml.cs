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

        public EditarClienteModel(MySqlConnection connection)
        {
            _connection = connection;
        }

        [BindProperty]
        public Cliente Cliente { get; set; } = new Cliente();

        public IActionResult OnGet(int id)
        {
            try
            {
                _connection.Open();

                string query = @"SELECT id, ci_nit, nombre, apellido, telefono, email, direccion, estado 
                                 FROM cliente 
                                 WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);

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
            // Trim and collapse spaces
            string NormalizeSpaces(string s) => s == null ? null : Regex.Replace(s.Trim(), "\\s+", " ");

            Cliente.Nombre = NormalizeSpaces(Cliente.Nombre);
            Cliente.Apellido = NormalizeSpaces(Cliente.Apellido);
            Cliente.CiNit = NormalizeSpaces(Cliente.CiNit);
            Cliente.Telefono = NormalizeSpaces(Cliente.Telefono);
            Cliente.Email = NormalizeSpaces(Cliente.Email);
            Cliente.Direccion = NormalizeSpaces(Cliente.Direccion);

            // Normalize name capitalization
            string NormalizeName(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return s;
                var parts = s.Split(' ');
                for (int i = 0; i < parts.Length; i++)
                {
                    var p = parts[i].ToLower();
                    parts[i] = char.ToUpper(p[0]) + (p.Length > 1 ? p.Substring(1) : "");
                }
                return string.Join(' ', parts);
            }

            Cliente.Nombre = NormalizeName(Cliente.Nombre);
            Cliente.Apellido = NormalizeName(Cliente.Apellido);

            // Re-validate model after normalization
            ModelState.Clear();
            TryValidateModel(Cliente, nameof(Cliente));

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Model-level validations (IValidatableObject on Cliente) already ran via TryValidateModel

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
                    cmd.Parameters.AddWithValue("@id", Cliente.Id);

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