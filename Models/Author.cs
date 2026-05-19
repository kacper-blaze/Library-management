using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lab10.Models;

[Table("authors")]
public class Author
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [Column(TypeName = "varchar(100)")]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "varchar(100)")]
    public string LastName { get; set; } = string.Empty;
    
    [Column(TypeName = "varchar(500)")]
    public string? Biography { get; set; }
    
    // Navigation property
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
