namespace TallerMecanicoScrum.Clases
{
    public class Servicio
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = "";

        public string Tipo { get; set; } = "";

        public string Descripcion { get; set; } = "";

        public decimal Precio { get; set; }

        public int DuracionEstimada { get; set; }

        public string UnidadDuracion { get; set; } = "";

        public bool Estado { get; set; } = true;
    }
}