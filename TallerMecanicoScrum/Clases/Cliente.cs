using System.ComponentModel.DataAnnotations;

namespace TallerMecanicoScrum.Clases
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(80, ErrorMessage = "El nombre no puede superar 80 caracteres")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(80, ErrorMessage = "El apellido no puede superar 80 caracteres")]
        public string Apellido { get; set; } = "";

        [Required(ErrorMessage = "El CI/NIT es obligatorio")]
        [StringLength(20, ErrorMessage = "El CI/NIT no puede superar 20 caracteres")]
        public string CiNit { get; set; } = "";

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20, ErrorMessage = "El teléfono no puede superar 20 caracteres")]
        [Phone(ErrorMessage = "El formato de teléfono no es válido")]
        public string Telefono { get; set; } = "";

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [StringLength(120, ErrorMessage = "El correo no puede superar 120 caracteres")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200, ErrorMessage = "La dirección no puede superar 200 caracteres")]
        public string Direccion { get; set; } = "";
        public bool Estado { get; set; } = true;

        public DateTime FechaRegistro { get; set; }
    }
}