using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProfileService.Api.Application.Dtos;
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

        public void createProfileAsync(ProfileUser user)
        {
            _context.ProfileUsers.Add(user);
        }

        public async void DeleteProfile(string id)
        {
            var profile = await GetProfileByIdAsync(id);
             _context.ProfileUsers.Remove(profile) ;
        }

        public async Task<ProfileUser> GetProfileByIdAsync(string id)
        {
            return await _context.ProfileUsers.FindAsync(id);
        }

        public async Task<ProfileUser> GetProfileByUsername(string username)
        {
            return await _context.ProfileUsers
             .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.Username == username);
        }

        public async Task<IEnumerable<ProfileUser>> GetProfilesAsync()
        {
            return await _context.ProfileUsers.Include(p => p.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void UpdateProfile(ProfileUser profile)
        {
            throw new System.NotImplementedException();
        }
    }
}