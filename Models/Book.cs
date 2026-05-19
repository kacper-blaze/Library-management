using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lab10.Models;

[Table("books")]
public class Book
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [Column(TypeName = "varchar(200)")]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ISBN { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "int")]
    public int PublicationYear { get; set; }
    
    [Required]
    [Column(TypeName = "varchar(50)")]
    public string Publisher { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "int")]
    public int TotalCopies { get; set; }
    
    [Required]
    [Column(TypeName = "int")]
    public int AvailableCopies { get; set; }
    
    // Foreign keys
    [Required]
    public int AuthorId { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    // Navigation properties
    [ForeignKey("AuthorId")]
    public Author Author { get; set; } = null!;
    
    [ForeignKey("CategoryId")]
    public Category Category { get; set; } = null!;
    
    public ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();
}
