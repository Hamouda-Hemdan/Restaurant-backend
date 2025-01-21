using Microsoft.AspNetCore.Identity;
using resturant1.Models.Entities;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Address { get; set; }
    public Gender Gender { get; set; }  
    public string PhoneNumber { get; set; }
}
