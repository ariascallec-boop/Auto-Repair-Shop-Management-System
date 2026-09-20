namespace TallerMecanicoScrum.Clases
{
    public class Vehiculo
    {
        public int Id { get; set; }

        public string Placa { get; set; } = "";

        public string Marca { get; set; } = "";

        public string Modelo { get; set; } = "";

        public int Anio { get; set; }

        public string Color { get; set; } = "";

        public string Tipo { get; set; } = "";

        public int Kilometraje { get; set; }

        public int ClienteId { get; set; }

        public bool Estado { get; set; } = true;

        public DateTime CreadoEn { get; set; }
    }
}