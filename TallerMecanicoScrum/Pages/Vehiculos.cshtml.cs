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
            // Backend:
            // aquí se realizará el borrado lógico
            // cambiando Estado = false.

            return RedirectToPage("/Vehiculos");
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
                Tipo = "Sedán",
                Kilometraje = 85000,
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
                Tipo = "SUV",
                Kilometraje = 62000,
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
                Tipo = "Sedán",
                Kilometraje = 105000,
                ClienteId = 3,
                Estado = true
            });
        }

        private void BuscarVehiculos()
        {
            List<Vehiculo> resultado = new List<Vehiculo>();

            for (int i = 0; i < ListaVehiculos.Count; i++)
            {
                if (
                    ListaVehiculos[i].Placa.Contains(
                        Buscar,
                        StringComparison.OrdinalIgnoreCase
                    ) ||

                    ListaVehiculos[i].Marca.Contains(
                        Buscar,
                        StringComparison.OrdinalIgnoreCase
                    ) ||

                    ListaVehiculos[i].Modelo.Contains(
                        Buscar,
                        StringComparison.OrdinalIgnoreCase
                    ) ||

                    ListaVehiculos[i].Tipo.Contains(
                        Buscar,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    resultado.Add(ListaVehiculos[i]);
                }
            }

            ListaVehiculos = resultado;
        }
    }
}