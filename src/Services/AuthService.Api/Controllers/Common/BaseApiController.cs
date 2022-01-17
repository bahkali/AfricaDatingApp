using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers.Common
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BaseApiController : ControllerBase
    {
    }
}
