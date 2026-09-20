using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TallerMecanicoScrum.Clases;

namespace TallerMecanicoScrum.Pages
{
    public class ClientesModel : PageModel
    {
        public List<Cliente> ListaClientes { get; set; } = new List<Cliente>();

        public string Buscar { get; set; } = "";

        public void OnGet(string buscar)
        {
            CargarClientes();

            Buscar = buscar ?? "";

            if (!string.IsNullOrWhiteSpace(Buscar))
            {
                BuscarClientes();
            }
        }

        public IActionResult OnPostEliminar(int id)
        {
            // Backend:
            // aquí se actualizará Estado = false
            // para realizar el borrado lógico.

            return RedirectToPage("/Clientes");
        }

        private void CargarClientes()
        {
            ListaClientes.Add(new Cliente
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Perez",
                CiNit = "1234567",
                Telefono = "70707070",
                Email = "juan.perez@gmail.com",
                Direccion = "Av. Blanco Galindo",
                Estado = true
            });

            ListaClientes.Add(new Cliente
            {
                Id = 2,
                Nombre = "Maria",
                Apellido = "Gomez",
                CiNit = "7654321",
                Telefono = "71717171",
                Email = "maria.gomez@gmail.com",
                Direccion = "Av. America",
                Estado = true
            });

            ListaClientes.Add(new Cliente
            {
                Id = 3,
                Nombre = "Pedro",
                Apellido = "Sanchez",
                CiNit = "4567890",
                Telefono = "72727272",
                Email = "pedro.sanchez@gmail.com",
                Direccion = "Av. Beijing",
                Estado = true
            });
        }

        private void BuscarClientes()
        {
            List<Cliente> resultado = new List<Cliente>();

            for (int i = 0; i < ListaClientes.Count; i++)
            {
                if (
                    ListaClientes[i].Nombre.Contains(
                        Buscar,
                        StringComparison.OrdinalIgnoreCase
                    ) ||

                    ListaClientes[i].Apellido.Contains(
                        Buscar,
                        StringComparison.OrdinalIgnoreCase
                    ) ||

                    ListaClientes[i].CiNit.Contains(
                        Buscar,
                        StringComparison.OrdinalIgnoreCase
                    ) ||

                    ListaClientes[i].Email.Contains(
                        Buscar,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    resultado.Add(ListaClientes[i]);
                }
            }

            ListaClientes = resultado;
        }
    }
}