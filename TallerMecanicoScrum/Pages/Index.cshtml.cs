using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TallerMecanicoScrum.Pages
{
    public class IndexModel : PageModel
    {
        public List<VehiculoResumen> Vehiculos { get; set; }
            = new List<VehiculoResumen>();

        public int TotalVehiculos { get; set; }

        public int TotalEnReparacion { get; set; }

        public int TotalListosEntrega { get; set; }

        public int TotalEntregasHoy { get; set; }

        public void OnGet()
        {
            CargarVehiculos();

            CalcularResumen();
        }

        private void CargarVehiculos()
        {
            Vehiculos.Add(new VehiculoResumen
            {
                Placa = "ABC-123",
                Cliente = "Juan Perez",
                Vehiculo = "Toyota Corolla",
                Estado = "listo_para_entrega",
                Responsable = "Carlos Rojas",
                FechaEntregaEstimada = DateTime.Today
            });

            Vehiculos.Add(new VehiculoResumen
            {
                Placa = "DEF-456",
                Cliente = "Maria Gomez",
                Vehiculo = "Hyundai Tucson",
                Estado = "en_prueba",
                Responsable = "Luis Martinez",
                FechaEntregaEstimada = DateTime.Today
            });

            Vehiculos.Add(new VehiculoResumen
            {
                Placa = "GHI-789",
                Cliente = "Pedro Sanchez",
                Vehiculo = "Chevrolet Sail",
                Estado = "en_reparacion",
                Responsable = "Ana Torres",
                FechaEntregaEstimada = DateTime.Today.AddDays(1)
            });

            Vehiculos.Add(new VehiculoResumen
            {
                Placa = "JKL-012",
                Cliente = "Laura Castro",
                Vehiculo = "Kia Sportage",
                Estado = "en_revision",
                Responsable = "Diego Ramirez",
                FechaEntregaEstimada = DateTime.Today.AddDays(2)
            });
        }

        private void CalcularResumen()
        {
            TotalVehiculos = Vehiculos.Count;

            TotalEnReparacion = 0;
            TotalListosEntrega = 0;
            TotalEntregasHoy = 0;

            for (int i = 0; i < Vehiculos.Count; i++)
            {
                if (Vehiculos[i].Estado == "en_reparacion")
                {
                    TotalEnReparacion++;
                }

                if (Vehiculos[i].Estado == "listo_para_entrega")
                {
                    TotalListosEntrega++;
                }

                if (Vehiculos[i].FechaEntregaEstimada.Date == DateTime.Today)
                {
                    TotalEntregasHoy++;
                }
            }
        }
    }

    public class VehiculoResumen
    {
        public string Placa { get; set; } = "";

        public string Cliente { get; set; } = "";

        public string Vehiculo { get; set; } = "";

        public string Estado { get; set; } = "";

        public string Responsable { get; set; } = "";

        public DateTime FechaEntregaEstimada { get; set; }
    }
}