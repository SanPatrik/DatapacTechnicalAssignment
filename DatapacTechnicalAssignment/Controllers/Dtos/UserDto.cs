using System.ComponentModel.DataAnnotations;

namespace DatapacTechnicalAssignment.Controllers.Dtos;

public class UserDto
{
    public string Name { get; set; }
    [EmailAddress]
    public string Email { get; set; }
}