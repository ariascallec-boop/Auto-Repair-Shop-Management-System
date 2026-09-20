namespace clases_Taller.Pages
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public int Quantity { get; set; }

        // Clave foránea y relación con Orden
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
