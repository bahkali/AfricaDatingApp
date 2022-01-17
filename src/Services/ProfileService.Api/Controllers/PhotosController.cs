using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileService.Api.Application.Persistence.Repositories.Interface;
using ProfileService.Api.Domain;
using ProfileService.Api.Domain.Entities;

namespace ProfileService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/Profile/[controller]")]
    public class PhotosController : ControllerBase
    {
        private IUserAccessor _userAccessor;
        private readonly ProfileDataContext _context;
        private readonly IPhotoAccessor _photoAccessor;

        public PhotosController(
            ProfileDataContext context, 
            IPhotoAccessor photoAccessor,
            IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
            _context = context;
            _photoAccessor = photoAccessor;
        }

        [HttpPost("/{id}")]
        public async Task<ActionResult<Photo>> AddProfilePhoto(string id, IFormFile File)
        {
            var profile = await _context.ProfileUsers.Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.Id == id);

            var photoUploadResult = await _photoAccessor.AddPhotoAsync(File);
            var photo = new Photo
            {
                Url = photoUploadResult.Url.ToString(),
                Id = photoUploadResult.PublicId
            };
            // make the picture main now
            if(!profile.Photos.Any(x => x.isMain)) photo.isMain = true;

            profile.Photos.Add(photo);
            var result = await _context.SaveChangesAsync() > 0;
            if (result) return Ok(photo);
            return BadRequest("Problem adding photo");
        }
    
        [HttpDelete("/{id}")]
        public async Task<ActionResult<Photo>> DeleteProfilePhoto(string id)
        {
            var profile = await _context.ProfileUsers.Include(p => p.Photos)
            .FirstOrDefaultAsync(x => x.Username == _userAccessor.GetUserName());
            if(profile == null) return null;

            var photo = profile.Photos.FirstOrDefault(x => x.Id == id);
            if (photo == null) return null;
            if(photo.isMain) return BadRequest("you cannot delete your main photo");

            var result = await _photoAccessor.DeletePhotoAsync(photo.Id);
            if(result == null) return BadRequest("Problem deleting photo from Cloundinary");

            profile.Photos.Remove(photo);
            return Ok(await _context.SaveChangesAsync());
        }
    }
}