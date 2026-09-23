namespace TallerMecanicoScrum.Clases
{
    public class OrdenTrabajo
    {
        public int Id { get; set; }

        public int VehiculoId { get; set; }

        public DateTime FechaIngreso { get; set; }

        public DateTime FechaEntregaEstimada { get; set; }

        public string Estado { get; set; } = "";

        public string Diagnostico { get; set; } = "";

        public DateTime? FechaNotificacion { get; set; }

        public string ObservacionesNotificacion { get; set; } = "";

        public bool Activo { get; set; } = true;
    }
}
