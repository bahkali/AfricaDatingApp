using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers.Common
{
    [ApiController]
    [Route("/[controller]")]
    public class BaseApiController : ControllerBase
    {
    }
}
