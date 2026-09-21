using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TallerMecanicoScrum.Clases;

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
            if (!ModelState.IsValid)
            {
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