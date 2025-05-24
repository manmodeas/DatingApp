
using API.Extensions;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public byte[] PasswordHash { get; set; } = [];
        public byte[] PasswordSalt { get; set; } = [];
        public DateOnly DateOfBirth { get; set; }
        public required string KnownAs { get; set; }
        public DateTime Created {  get; set; }
        public DateTime LastActive { get; set; }
        public required string Gender { get; set; }
        public string? Introductions {  get; set; }
        public string? Interests { get; set; }
        public string? LookingFor { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public List<Photo> Photos { get; set; } = [];   //This is realtional entityframework thing,,, where entity framework will identiy and create "photo" table for us.... which can be asseccible from "Users.Photos" 

        public int GetAge()
        {
            return DateOfBirth.CalculateAge();
        }
    }
}
