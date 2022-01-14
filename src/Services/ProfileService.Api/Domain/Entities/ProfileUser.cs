using System;
using System.Collections.Generic;

namespace ProfileService.Api.Domain.Entities
{
    public class ProfileUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string  City { get; set; }
        public string Country { get; set; }
        public string CountryOrigin { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public DateTime DateOfBirth { get; set; } 
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;

        //public int GetAge() => DateOfBirth.CalculateAge();
    }
}