using System.ComponentModel.DataAnnotations;

namespace DatapacTechnicalAssignment.Controllers.Dtos;

public class BookDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [Required]
    [StringLength(100)]
    public string Author { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; set; }
    
}
