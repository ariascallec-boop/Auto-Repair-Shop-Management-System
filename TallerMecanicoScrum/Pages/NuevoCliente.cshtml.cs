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
            // Normalización (igual que en EditarCliente, para que crear y editar
            // se comporten igual con espacios extra, mayúsculas, etc.)
            Cliente.Nombre = NormalizarEspacios(Cliente.Nombre);
            Cliente.Apellido = NormalizarEspacios(Cliente.Apellido);
            Cliente.CiNit = NormalizarEspacios(Cliente.CiNit);
            Cliente.Telefono = NormalizarEspacios(Cliente.Telefono);
            Cliente.Email = NormalizarEspacios(Cliente.Email);
            Cliente.Direccion = NormalizarEspacios(Cliente.Direccion);

            Cliente.Nombre = NormalizarNombre(Cliente.Nombre);
            Cliente.Apellido = NormalizarNombre(Cliente.Apellido);

            // Revalida sobre valores YA normalizados
            ModelState.Clear();
            TryValidateModel(Cliente, nameof(Cliente));

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!Regex.IsMatch(Cliente.CiNit, "^[0-9]+$"))
            {
                ModelState.AddModelError("Cliente.CiNit", "El CI/NIT debe contener solo dígitos");
                return Page();
            }

            if (!Validaciones.SoloLetras(Cliente.Nombre))
            {
                ModelState.AddModelError("Cliente.Nombre", "El nombre solo puede contener letras y espacios");
                return Page();
            }

            if (!Validaciones.SoloLetras(Cliente.Apellido))
            {
                ModelState.AddModelError("Cliente.Apellido", "El apellido solo puede contener letras y espacios");
                return Page();
            }

            if (!Validaciones.ValidarEmail(Cliente.Email))
            {
                ModelState.AddModelError("Cliente.Email", "El correo electrónico no es válido (nombreapellido@gmail.com) ");
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
                    cmd.Parameters.AddWithValue("@nombre", Cliente.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", Cliente.Apellido);
                    cmd.Parameters.AddWithValue("@ci_nit", Cliente.CiNit);
                    cmd.Parameters.AddWithValue("@telefono", Cliente.Telefono);
                    cmd.Parameters.AddWithValue("@email", Cliente.Email);
                    cmd.Parameters.AddWithValue("@direccion", Cliente.Direccion);
                    cmd.Parameters.AddWithValue("@estado", Cliente.Estado);

                    cmd.ExecuteNonQuery();
                }

                return RedirectToPage("/Clientes");
            }
            catch (MySqlException ex) when (ex.Number == 1062) // clave duplicada (ci_nit UNIQUE)
            {
                ModelState.AddModelError(
                    "Cliente.CiNit",
                    "Ya existe un cliente registrado con ese CI/NIT."
                );
                return Page();
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", "Error de MySQL: " + ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar el cliente: " + ex.Message);
                return Page();
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
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