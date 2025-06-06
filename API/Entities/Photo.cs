﻿using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")]   //It Tells EntityFramework to name table "Photos" rather than "Photo" which is the class name 
    public class Photo
    {
        public int Id { get; set; }
        public required string Url {  get; set; }
        public bool IsMain { get; set; }
        public string? PublicId { get; set; }
        public bool IsApproved {  get; set; }

        //Navigation Properties
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; } = null!;
    }
}
