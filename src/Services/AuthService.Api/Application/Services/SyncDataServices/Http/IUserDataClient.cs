using System.Threading.Tasks;
using AuthService.Api.Application.Dtos;

namespace AuthService.Api.Application.Services.SyncDataServices.Http
{
    public interface IUserDataClient
    {
         Task SendUserToProfile(UserSendDto user);
    }
}