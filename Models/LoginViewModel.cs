using System.ComponentModel.DataAnnotations;

namespace GestionPedidos.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inv�lido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contrase�a es requerida")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}