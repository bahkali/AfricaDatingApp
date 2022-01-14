using System.Collections.Generic;
using System.Threading.Tasks;
using ProfileService.Api.Domain.Entities;

namespace ProfileService.Api.Application.Persistence.Repositories.Interface
{
    public interface IProfileRepository
    {
         Task<IEnumerable<ProfileUser>> GetProfiles();
         Task<ProfileUser> GetProfile(string id);
         Task createProfile(AppUser user);
         Task<bool> UpdateProfile(ProfileUser profile);
         Task<bool> DeleteProfile(string id);
    }
}