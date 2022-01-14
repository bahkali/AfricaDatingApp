using System.Collections.Generic;
using System.Threading.Tasks;
using ProfileService.Api.Application.Persistence.Repositories.Interface;
using ProfileService.Api.Domain;
using ProfileService.Api.Domain.Entities;

namespace ProfileService.Api.Application.Persistence.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ProfileDataContext _context;

        public ProfileRepository(ProfileDataContext context)
        {
            _context = context;
        }
        public Task createProfile(AppUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteProfile(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<ProfileUser> GetProfile(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<ProfileUser>> GetProfiles()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateProfile(ProfileUser profile)
        {
            throw new System.NotImplementedException();
        }
    }
}