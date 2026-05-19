using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lab10.Models;

[Table("members")]
public class Member
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [Column(TypeName = "varchar(100)")]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "varchar(100)")]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "varchar(100)")]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Column(TypeName = "date")]
    public DateTime MembershipDate { get; set; } = DateTime.Now;
    
    [Required]
    [Column(TypeName = "varchar(200)")]
    public string Address { get; set; } = string.Empty;
    
    // Navigation property
    public ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();
}
