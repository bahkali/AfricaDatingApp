using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Api.Application.Dtos;
using ProfileService.Api.Application.Persistence.Repositories.Interface;
using ProfileService.Api.Controllers.Common;
using ProfileService.Api.Domain.Entities;

namespace ProfileService.Api.Controllers
{
    public class ProfileController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoAccessor _photoAccessor;
        private readonly IProfileRepository _profileRepository;

        public ProfileController(
            IPhotoAccessor photoAccessor,
            IProfileRepository profileRepository,
            IMapper mapper
        )
        {
            _mapper = mapper;
            _photoAccessor = photoAccessor;
            _profileRepository = profileRepository;
        }
        [HttpPost("createProfile")]
        public async Task<ActionResult> createProfile(CreateProfileDto user)
        {
            var profile = _mapper.Map<ProfileUser>(user);
            _profileRepository.createProfileAsync(profile);
            return Ok(await _profileRepository.SaveAllAsync());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProfile(string id, [FromBody] UpdateProfileDto profile)
        {
            var existingProfile = await _profileRepository.GetProfileByIdAsync(id);
            if(existingProfile == null) return BadRequest("Profile doesn't exist");
            _mapper.Map(profile, existingProfile);
            return Ok(await _profileRepository.SaveAllAsync());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfileDto>>> GetProfiles()
        {
            var profiles = await _profileRepository.GetProfilesAsync();
            var profileToReturn = _mapper.Map<IEnumerable<ProfileDto>>(profiles);
            return Ok(profileToReturn);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileDto>> GetProfile(string id)
        {
            var profile = await _profileRepository.GetProfileByIdAsync(id);
            return _mapper.Map<ProfileDto>(profile);
        }

        [HttpGet("ByUsername/{username}")]
        public async Task<ActionResult<ProfileDto>> GetProfileByUsername(string username)
        {
            var profile = await _profileRepository.GetProfileByUsername(username);
            return _mapper.Map<ProfileDto>(profile);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteProfile(string id)
        {
            _profileRepository.DeleteProfile(id);
            return Ok("Profile deleted");
        }

        [HttpPost]
        public ActionResult TestInboundConnection(CreateProfileDto user)
        {
            Console.WriteLine($"Inbound post # Profile Service {user.Email.ToString()}");
            return Ok("Inbound test from Profile service ");
        }
    }
}