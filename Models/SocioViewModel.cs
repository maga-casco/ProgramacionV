using System.ComponentModel.DataAnnotations;

namespace ProgramacionV.Models
{
    public class SocioViewModel
    {

        public int Id { get; set; }


        [Required(ErrorMessage = "Ingrese el nombre")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "Ingrese el apellido")]
        public string Apellido { get; set; }


        [Required(ErrorMessage = "Ingrese el DNI")]
        public string DNI { get; set; }


        [Required(ErrorMessage = "Ingrese la fecha de nacimiento")]
        public DateTime? FechaNacimiento { get; set; }


        public string? Telefono { get; set; }


        public string? Email { get; set; }


        public string? Direccion { get; set; }


        public DateTime FechaAlta { get; set; }


        public bool Activo { get; set; }

    }
}