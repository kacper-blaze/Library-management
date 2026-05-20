using System.ComponentModel.DataAnnotations;

namespace lab10.Models;

public class AddUserViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Role")]
    public string Role { get; set; } = "User";
}
