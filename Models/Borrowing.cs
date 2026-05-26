using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lab10.Models;

[Table("borrowings")]
public class Borrowing
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int BookId { get; set; }
    
    [Required]
    public int MemberId { get; set; }
    
    [Required]
    [Column(TypeName = "date")]
    public DateTime BorrowDate { get; set; } = DateTime.Now;
    
    [Column(TypeName = "date")]
    public DateTime DueDate { get; set; }
    
    [Column(TypeName = "date")]
    public DateTime? ReturnDate { get; set; }
    
    // Navigation properties
    [ForeignKey("BookId")]
    public Book? Book { get; set; }
    
    [ForeignKey("MemberId")]
    public Member? Member { get; set; }
}
