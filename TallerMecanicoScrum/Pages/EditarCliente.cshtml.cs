using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class EditarClienteModel : PageModel
    {
        [BindProperty]
        public Cliente Cliente { get; set; } = new Cliente();

        public void OnGet(int id)
        {
            // Datos temporales.
            // Después vendrán desde la base de datos según el id.

            Cliente = new Cliente
            {
                Id = id,
                Nombre = "Juan",
                Apellido = "Perez",
                CiNit = "1234567",
                Telefono = "70707070",
                Direccion = "Av. Blanco Galindo",
                Estado = true
            };
        }

        public IActionResult OnPost()
        {
            // Después aquí se actualizará el cliente en la base de datos.

            return RedirectToPage("/Clientes");
        }
    }
}
