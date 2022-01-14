using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Api.Application.Dtos;
using ProfileService.Api.Application.Persistence.Repositories.Interface;
using ProfileService.Api.Controllers.Common;
using ProfileService.Api.Domain;
using ProfileService.Api.Domain.Entities;

namespace ProfileService.Api.Controllers
{
    public class ProfileController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ProfileDataContext _context;
        private readonly IPhotoAccessor _photoAccessor;

        public ProfileController(
            ProfileDataContext context, 
            IPhotoAccessor photoAccessor,
            IMapper mapper
        )
        {
            _mapper = mapper;
            _context = context;
            _photoAccessor = photoAccessor;
        }
        [HttpPost("createProfile")]
        public async Task<ActionResult> createProfile(CreateProfileDto user)
        {
            var profile = _mapper.Map<ProfileUser>(user);
            _context.ProfileUsers.Add(profile);
            return Ok(await _context.SaveChangesAsync() > 0);
        }

        [HttpPost]
        public ActionResult TestInboundConnection(CreateProfileDto user)
        {
            Console.WriteLine($"Inbound post # Profile Service {user.Email.ToString()}");
            return Ok("Inbound test from Profile service ");
        }
    }
}