using Microsoft.AspNetCore.Http;
using ProfileService.Api.Application.Persistence.Repositories.Interface;

namespace ProfileService.Api.Application.Services.Users
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext.User.Identity.Name;
        }
    }
}