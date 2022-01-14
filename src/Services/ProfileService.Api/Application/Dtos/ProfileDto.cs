using System;
using System.Collections.Generic;
using ProfileService.Api.Domain.Entities;

namespace ProfileService.Api.Application.Dtos
{
    public class ProfileDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string photoUrl { get; set; }
        public int Age { get; set; }
        public string DisplayName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string  City { get; set; }
        public string Country { get; set; }
        public string CountryOrigin { get; set; }
        public ICollection<PhotoDto> Photos { get; set; }
        public DateTime DateOfBirth { get; set; } 
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
    }
}