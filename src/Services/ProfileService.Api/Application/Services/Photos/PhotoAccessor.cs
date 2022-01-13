using System;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ProfileService.Api.Application.Persistence.Repositories.Interface;

namespace ProfileService.Api.Application.Services.Photos
{
    public class PhotoAccessor : IPhotoAccessor
    {
        private readonly Cloudinary _cloudinary;
        public PhotoAccessor(IOptions<CloudinarySetting> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }
        public async Task<PhotoUploadResult> AddPhotoAsync(IFormFile File)
        {
            if(File.Length > 0)
            {
                await using var stream = File.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(File.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                };
                var uploadResult =  _cloudinary.Upload(uploadParams);

                if (uploadResult.Error != null) { throw new Exception(uploadResult.Error.Message); }

                return new PhotoUploadResult
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.SecureUrl.ToString(),
                };
            }
            return null;
        }

        public async Task<string> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok" ? result.Result : null;
        }
    }
}