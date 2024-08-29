using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatapacTechnicalAssignment.Models;

[Table("loans")]
public class Loan
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid BookId { get; set; }
    public Book Book { get; set; }

    public DateTime BorrowedDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
    public DateTime DueDate { get; set; }
}
