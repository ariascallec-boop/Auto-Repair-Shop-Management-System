namespace clases_Taller.Pages
{
    public class Vehicle : BaseEntity
    {
        public string Plate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public string Type { get; set; }
        public string Status { get; set; } // Estado físico al ingresar

        // Clave foránea y relación con Cliente
        public int ClientId { get; set; }
        public Client Client { get; set; }

        // Relación: Un vehículo puede tener múltiples órdenes
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
