﻿
using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUser : IdentityUser<int>
    {
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

        public List<UserLike> LikedByUser { get; set; } = [];
        public List<UserLike> LikedUser { get; set; } = [];

        public List<Message> MessagesSent { get; set; } = [];
        public List<Message> MessagesReceived { get; set; } = [];

        public ICollection<AppUserRole> UserRoles { get; set; } = [];   //since we are using Icollection AppRole Class we can also use List 

    }
}
