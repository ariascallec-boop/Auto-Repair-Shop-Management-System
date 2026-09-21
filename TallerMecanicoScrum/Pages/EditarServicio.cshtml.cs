using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class EditarServicioModel : PageModel
    {
        [BindProperty]
        public Servicio Servicio { get; set; } = new Servicio();

        public void OnGet(int id)
        {
            // TEMPORAL:
            // Luego se obtendrá desde la base de datos por Id.

            Servicio = new Servicio
            {
                Id = id,
                Nombre = "Cambio de aceite",
                Tipo = "preventivo",
                Descripcion = "Cambio de aceite y revisión general",
                Precio = 180,
                DuracionEstimada = 45,
                UnidadDuracion = "Minutos",
                Estado = true
            };
        }

        public IActionResult OnPost()
        {
            // BACKEND:
            // Luego aquí se actualizará
            // el servicio en la base de datos.

            return RedirectToPage("/Servicios");
        }
    }
}