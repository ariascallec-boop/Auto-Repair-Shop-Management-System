using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class ServiciosModel : PageModel
    {
        public List<Servicio> ListaServicios { get; set; }
            = new List<Servicio>();

        public string Buscar { get; set; } = "";

        public void OnGet(string buscar)
        {
            CargarServicios();

            Buscar = buscar ?? "";

            if (!string.IsNullOrWhiteSpace(Buscar))
            {
                BuscarServicios();
            }
        }

        private void CargarServicios()
        {
            // TEMPORAL:
            // Estos datos se usan mientras no se conecte la base de datos.

            ListaServicios.Add(new Servicio
            {
                Id = 1,
                Nombre = "Cambio de aceite",
                Tipo = "preventivo",
                Descripcion = "Cambio de aceite y revisión general",
                Precio = 180,
                DuracionEstimada = 45,
                UnidadDuracion = "Minutos",
                Estado = true
            });

            ListaServicios.Add(new Servicio
            {
                Id = 2,
                Nombre = "Revisión de frenos",
                Tipo = "revision_simple",
                Descripcion = "Inspección del sistema de frenos",
                Precio = 120,
                DuracionEstimada = 2,
                UnidadDuracion = "Horas",
                Estado = true
            });

            ListaServicios.Add(new Servicio
            {
                Id = 3,
                Nombre = "Reparación de motor",
                Tipo = "correctivo",
                Descripcion = "Diagnóstico y reparación del motor",
                Precio = 800,
                DuracionEstimada = 2,
                UnidadDuracion = "Dias",
                Estado = true
            });
        }

        private void BuscarServicios()
        {
            List<Servicio> resultado = new List<Servicio>();

            for (int i = 0; i < ListaServicios.Count; i++)
            {
                if (
                    ListaServicios[i].Nombre.Contains(
                        Buscar,
                        StringComparison.OrdinalIgnoreCase
                    ) ||

                    ListaServicios[i].Tipo.Contains(
                        Buscar,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    resultado.Add(ListaServicios[i]);
                }
            }

            ListaServicios = resultado;
        }

        public IActionResult OnPostEliminar(int id)
        {
            // BACKEND:
            // Luego se buscará el servicio por Id
            // y se cambiará Estado = false
            // para realizar borrado lógico.

            return RedirectToPage("/Servicios");
        }
    }
}