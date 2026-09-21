using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
namespace TallerMecanicoScrum.Pages
{
    public class IndexModel : PageModel
    {
        private readonly MySqlConnection _connection;

        // Inyección de dependencias configurada desde Program.cs
        public IndexModel(MySqlConnection connection)
        {
            _connection = connection;
        }

        public List<VehiculoResumen> Vehiculos { get; set; } = new List<VehiculoResumen>();

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
            Vehiculos.Clear();

            try
            {
                _connection.Open();

                string query = @"SELECT placa, cliente, vehiculo, estado, responsable, fecha_entrega_estimada 
                                 FROM vw_resumen_inicio";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Vehiculos.Add(new VehiculoResumen
                            {
                                Placa = reader["placa"]?.ToString() ?? "",
                                Cliente = reader["cliente"]?.ToString() ?? "",
                                Vehiculo = reader["vehiculo"]?.ToString() ?? "",
                                Estado = reader["estado"]?.ToString() ?? "",
                                Responsable = reader["responsable"]?.ToString() ?? "",
                                FechaEntregaEstimada = reader["fecha_entrega_estimada"] != DBNull.Value
                                                       ? Convert.ToDateTime(reader["fecha_entrega_estimada"])
                                                       : DateTime.MinValue
                            });
                        }
                    }
                }
            }
            finally
            {
                _connection.Close();
            }
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