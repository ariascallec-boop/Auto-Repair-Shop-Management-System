namespace clases_Taller.Pages
{
    public class Mechanic : BaseEntity
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Document { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        // Relación con Órdenes
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
