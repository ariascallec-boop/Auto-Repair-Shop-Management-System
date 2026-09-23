namespace TallerMecanicoScrum.Clases
{
    public class OrdenServicio
    {
        public int Id { get; set; }
        public int OrdenTrabajoId { get; set; }
        public int ServicioId { get; set; } // Sin el "?"
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal { get; set; }
    }
}
