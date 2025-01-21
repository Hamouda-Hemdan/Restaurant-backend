using resturant1.Models.Entities;

namespace resturant1.Models.Dto
{
    public class UserProfileUpdateDto
    {
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
