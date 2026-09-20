using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class NuevoClienteModel : PageModel
    {
        [BindProperty]
        public Cliente Cliente { get; set; } = new Cliente();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            Cliente.Estado = true;

            // Después aquí se guardará en la base de datos.

            return RedirectToPage("/Clientes");
        }
    }
}
