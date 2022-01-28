using AuthService.Api.Controllers.Common;
using AuthService.Api.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Api.Controllers
{

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WeatherForecastController : BaseApiController
    {

        [HttpGet]
        public ActionResult  WeatherForecast()
        {
            var rng = new Random();
            return Ok($"Hello from WeatherForecast");
        }
    }
}
