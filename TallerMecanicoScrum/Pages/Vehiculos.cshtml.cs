using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class VehiculosModel : PageModel
    {
        public List<Vehiculo> ListaVehiculos { get; set; } = new List<Vehiculo>();
        public string Buscar { get; set; } = "";

        public void OnGet(string buscar)
        {
            CargarVehiculos();

            Buscar = buscar ?? "";

            if (!string.IsNullOrWhiteSpace(Buscar))
            {
                BuscarVehiculos();
            }
        }
        public IActionResult OnPostEliminar(int id)
        {
            // Más adelante aquí se hará el borrado lógico:
            // Estado = false

            return RedirectToPage("/Vehiculos");
        }
        private void BuscarVehiculos()
        {
            ListaVehiculos = ListaVehiculos
                .Where(v =>
                    v.Placa.Contains(Buscar, StringComparison.OrdinalIgnoreCase) ||
                    v.Marca.Contains(Buscar, StringComparison.OrdinalIgnoreCase) ||
                    v.Modelo.Contains(Buscar, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        private void CargarVehiculos()
        {
            ListaVehiculos.Add(new Vehiculo
            {
                Id = 1,
                Placa = "ABC-123",
                Marca = "Toyota",
                Modelo = "Corolla",
                Anio = 2020,
                Color = "Blanco",
                ClienteId = 1,
                Estado = true
            });
            ListaVehiculos.Add(new Vehiculo
            {
                Id = 2,
                Placa = "DEF-456",
                Marca = "Hyundai",
                Modelo = "Tucson",
                Anio = 2021,
                Color = "Negro",
                ClienteId = 2,
                Estado = true
            });

            ListaVehiculos.Add(new Vehiculo
            {
                Id = 3,
                Placa = "GHI-789",
                Marca = "Chevrolet",
                Modelo = "Sail",
                Anio = 2019,
                Color = "Rojo",
                ClienteId = 3,
                Estado = true
            });

        }
    }
}
