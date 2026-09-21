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
            Cliente = new Cliente
            {
                Id = id,
                Nombre = "Juan",
                Apellido = "Perez",
                CiNit = "1234567",
                Telefono = "70707070",
                Email = "juan.perez@gmail.com",
                Direccion = "Av. Blanco Galindo",
                Estado = true
            };
        }

        public IActionResult OnPost()
        {
            // Backend:
            // aquí se actualizará el cliente en la base de datos.

            return RedirectToPage("/Clientes");
        }
    }
}