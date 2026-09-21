using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class EditarVehiculoModel : PageModel
    {
        [BindProperty]
        public Vehiculo Vehiculo { get; set; } = new Vehiculo();

        public void OnGet(int id)
        {
            // Datos temporales
            // Después vendrán desde la base de datos según el id.

            Vehiculo = new Vehiculo
            {
                Id = id,
                Placa = "ABC-123",
                Marca = "Toyota",
                Modelo = "Corolla",
                Anio = 2020,
                Color = "Blanco",
                Tipo = "Sedán",
                Kilometraje = 85000,
                ClienteId = 1,
                Estado = true
            };
        }

        public IActionResult OnPost()
        {
            // Backend:
            // aquí se actualizará el vehículo en la base de datos.

            return RedirectToPage("/Vehiculos");
        }
    }
}