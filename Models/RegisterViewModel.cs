using System.ComponentModel.DataAnnotations;

namespace lab10.Models;

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "The password and confirmation must match.")]
    public string ConfirmPassword { get; set; }
}