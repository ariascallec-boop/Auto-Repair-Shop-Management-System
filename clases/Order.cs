namespace clases_Taller.Pages
{
    public class Order : BaseEntity
    {
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string Status { get; set; } // Estado del pedido (ej. "Pendiente", "Completado")
        public string PaymentStatus { get; set; } // "Pendiente", "Pagado"
        public decimal Balance { get; set; }
        public string TestResult { get; set; } // "Aprobado", "Reprobado"
        public DateTime? TestDate { get; set; }
        public bool NotificationStatus { get; set; }
        public DateTime? DeliveryDate { get; set; }

        // Clave foránea y relación con Vehículo
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        // Relaciones: Una orden agrupa servicios, productos y mecánicos
        public ICollection<Service> Services { get; set; } = new List<Service>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Mechanic> Mechanics { get; set; } = new List<Mechanic>();
    }
}
