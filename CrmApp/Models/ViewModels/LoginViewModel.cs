using System.ComponentModel.DataAnnotations;

namespace CrmApp.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Zadejte přihlašovací jméno.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Zadejte heslo.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
