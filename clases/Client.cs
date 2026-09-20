namespace clases_Taller.Pages
{
    public class Client : BaseEntity
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Ci { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        // Relación: Un cliente tiene varios vehículos
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
