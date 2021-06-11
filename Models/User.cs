using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório.")]
        [MaxLength(20, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres.")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório.")]
        [MaxLength(20, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres.")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres.")]
        public string Password { get; set; }

        public string Role { get; set; } //Cargo do usuário
    }
}