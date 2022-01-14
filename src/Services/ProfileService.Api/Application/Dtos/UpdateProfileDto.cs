using System;

namespace ProfileService.Api.Application.Dtos
{
    public class UpdateProfileDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Gender { get; set; }
        public string Bio { get; set; }
        public string  City { get; set; }
        public string Country { get; set; }
        public string CountryOrigin { get; set; }
        public DateTime DateOfBirth { get; set; } 
    }
}