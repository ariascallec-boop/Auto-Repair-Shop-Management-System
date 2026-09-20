using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class NuevoVehiculoModel : PageModel
    {
        [BindProperty]
        public Vehiculo Vehiculo { get; set; } = new Vehiculo();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            Vehiculo.Estado = true;

            // Después aquí se guardará en la base de datos.

            return RedirectToPage("/Vehiculos");
        }
    }
}
