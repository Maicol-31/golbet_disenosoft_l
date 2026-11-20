using System.ComponentModel.DataAnnotations;

namespace GolBet.Web.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [MaxLength(100)]
    [Display(Name = "Nombre completo")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "Mínimo 6 caracteres (1 mayúscula, 1 minúscula y 1 número)")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Confirme la contraseña")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = null!;
}