using System;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Api.Controllers.Common;

namespace ProfileService.Api.Controllers
{
    public class ProfileController : BaseApiController
    {
        public ProfileController()
        {
            
        }
        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("Inbound post # Profile Service");
            return Ok("Inbound test from Profile service ");
        }
    }
}