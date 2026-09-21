using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoScrum.Clases;
using System.Text.RegularExpressions;

namespace TallerMecanicoScrum.Pages
{
    public class NuevoClienteModel : PageModel
    {
        private readonly MySqlConnection _connection;

        public NuevoClienteModel(MySqlConnection connection)
        {
            _connection = connection;
        }

        [BindProperty]
        public Cliente Cliente { get; set; } = new Cliente();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Trim inputs (defensive)
            Cliente.Nombre = Cliente.Nombre?.Trim();
            Cliente.Apellido = Cliente.Apellido?.Trim();
            Cliente.CiNit = Cliente.CiNit?.Trim();
            Cliente.Telefono = Cliente.Telefono?.Trim();
            Cliente.Email = Cliente.Email?.Trim();
            Cliente.Direccion = Cliente.Direccion?.Trim();

            // Server-side additional validations
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!string.IsNullOrWhiteSpace(Cliente.CiNit) && !Regex.IsMatch(Cliente.CiNit, "^[0-9]+$"))
            {
                ModelState.AddModelError("Cliente.CiNit", "El CI/NIT debe contener solo dígitos");
                return Page();
            }

            Cliente.Estado = true;

            try
            {
                _connection.Open();

                string query = @"INSERT INTO cliente (nombre, apellido, ci_nit, telefono, email, direccion, estado) 
                                 VALUES (@nombre, @apellido, @ci_nit, @telefono, @email, @direccion, @estado)";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@nombre", Cliente.Nombre ?? "");
                    cmd.Parameters.AddWithValue("@apellido", Cliente.Apellido ?? "");
                    cmd.Parameters.AddWithValue("@ci_nit", Cliente.CiNit ?? "");
                    cmd.Parameters.AddWithValue("@telefono", Cliente.Telefono ?? "");
                    cmd.Parameters.AddWithValue("@email", Cliente.Email ?? "");
                    cmd.Parameters.AddWithValue("@direccion", Cliente.Direccion ?? "");
                    cmd.Parameters.AddWithValue("@estado", Cliente.Estado);

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