
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
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public required string Gender { get; set; }
        public string? Introduction {  get; set; }
        public string? Interests { get; set; }
        public string? LookingFor { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        //Photos is related entity of AppUser
        public List<Photo> Photos { get; set; } = [];   //This is realtional entityframework thing,,, where entity framework will identiy and create "photo" table for us.... which can be asseccible from "Users.Photos" 

        ////Because of this rather than getiing required select properties entity framework gets full appuser
        ////To Fix this we need to do this in AutoMapper
        //public int GetAge()
        //{
        //    return DateOfBirth.CalculateAge();
        //}
    }
}
