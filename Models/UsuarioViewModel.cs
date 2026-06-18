using System.ComponentModel.DataAnnotations;

namespace ProgramacionV.Models
{
    public class UsuarioViewModel
    {
        //public string Username { get; set; }
        //public string Password { get; set; }

        [Required(ErrorMessage = "Ingrese un nombre de usuario")]
        [Display(Name = "Usuario")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Ingrese una contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

    }
}
