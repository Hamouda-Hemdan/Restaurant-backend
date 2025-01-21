using resturant1.Models.Entities;
using System.ComponentModel.DataAnnotations;

public class UserRegisterModel
{
    [Required]
    [MinLength(1)]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    public string Address { get; set; }

    public DateTime? BirthDate { get; set; }

    [Required]
    public Gender Gender { get; set; }

    public string PhoneNumber { get; set; }
}
