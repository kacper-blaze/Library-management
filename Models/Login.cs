using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lab10.Models;

[Table("loginy")]
public class Login
{
    public string Role { get; set; }  // "Admin" lub "User"
    public string ApiToken { get; set; }  // token dla REST API
    
    [Key]
    public int Id { get; set; }
    
    [Required]
    [Column(TypeName = "varchar(100)")]
    public string Username { get; set; }
    
    [Required]
    [Column(TypeName = "varchar(255)")]
    public string Password { get; set; } 
}