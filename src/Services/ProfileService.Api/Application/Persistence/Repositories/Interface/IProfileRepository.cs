using System.Collections.Generic;
using System.Threading.Tasks;
using ProfileService.Api.Application.Dtos;
using ProfileService.Api.Domain.Entities;

namespace ProfileService.Api.Application.Persistence.Repositories.Interface
{
    public interface IProfileRepository
    {
         Task<IEnumerable<ProfileUser>> GetProfilesAsync();
         Task<ProfileUser> GetProfileByUsername(string username);
         Task<ProfileUser> GetProfileByIdAsync(string id);
         void createProfileAsync(ProfileUser user);
         void UpdateProfile(ProfileUser profile);
         void DeleteProfile(string id);
         Task<bool> SaveAllAsync();
    }
}