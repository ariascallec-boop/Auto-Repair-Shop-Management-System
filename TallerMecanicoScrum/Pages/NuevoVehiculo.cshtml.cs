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

            // Backend:
            // aquí se guardará el vehículo en la base de datos.

            return RedirectToPage("/Vehiculos");
        }
    }
}