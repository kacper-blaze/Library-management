using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lab10.Models;

[Table("categories")]
public class Category
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [Column(TypeName = "varchar(50)")]
    public string Name { get; set; } = string.Empty;
    
    [Column(TypeName = "varchar(500)")]
    public string? Description { get; set; }
    
    // Navigation property
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
