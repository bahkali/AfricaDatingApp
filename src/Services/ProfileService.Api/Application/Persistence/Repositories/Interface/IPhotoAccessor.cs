using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ProfileService.Api.Application.Persistence.Repositories.Interface
{
    public interface IPhotoAccessor
    {
        Task<PhotoUploadResult> AddPhotoAsync(IFormFile File);
        Task<string> DeletePhotoAsync(string publicId);
    }
}