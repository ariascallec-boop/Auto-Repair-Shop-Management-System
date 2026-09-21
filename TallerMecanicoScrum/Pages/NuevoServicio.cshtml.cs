using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class NuevoServicioModel : PageModel
    {
        [BindProperty]
        public Servicio Servicio { get; set; } = new Servicio();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            Servicio.Estado = true;

            // Backend:
            // aquí se guardará el servicio en la base de datos.

            return RedirectToPage("/Servicios");
        }
    }
}