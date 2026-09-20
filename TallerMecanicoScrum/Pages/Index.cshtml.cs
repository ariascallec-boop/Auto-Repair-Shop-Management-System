using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace TallerMecanicoScrum.Pages
{
    
    public class IndexModel : PageModel
    {
        var Builder
        public List<VehiculoResumen> Vehiculos { get; set; }= new List<VehiculoResumen>();

        public void OnGet()
        {

            Vehiculos.Add(new VehiculoResumen
            {
                Placa = "ABC-123",
                Cliente = "Juan Perez",
                Vehiculo = "Toyota Corolla",
                Estado = "Listo",
                Responsable = "Carlos Rojas",
                HoraEntrega = "10:00"
            });

            Vehiculos.Add(new VehiculoResumen
            {
                Placa = "DEF-456",
                Cliente = "Maria Gomez",
                Vehiculo = "Hyundai Tucson",
                Estado = "En prueba",
                Responsable = "Luis Martinez",
                HoraEntrega = "11:30"
            });

            Vehiculos.Add(new VehiculoResumen
            {
                Placa = "GHI-789",
                Cliente = "Pedro Sanchez",
                Vehiculo = "Chevrolet Sail",
                Estado = "En reparacion",
                Responsable = "Ana Torres",
                HoraEntrega = "14:00"
            });

            Vehiculos.Add(new VehiculoResumen
            {
                Placa = "JKL-012",
                Cliente = "Laura Castro",
                Vehiculo = "Kia Sportage",
                Estado = "En diagnostico",
                Responsable = "Diego Ramirez",
                HoraEntrega = "16:00"
            });
        }
    }


    public class VehiculoResumen
    {
        public string Placa { get; set; } = "";
        public string Cliente { get; set; } = "";
        public string Vehiculo { get; set; } = "";
        public string Estado { get; set; } = "";
        public string Responsable { get; set; } = "";
        public string HoraEntrega { get; set; } = "";
    }

}
