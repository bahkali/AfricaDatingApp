using AuthService.Api.Controllers.Common;
using AuthService.Api.Application.Dtos.Requests;
using AuthService.Api.Application.Dtos.Responses;
using AuthService.Api.Application.Services;
using AuthService.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Api.Application.Services.SyncDataServices.Http;
using AuthService.Api.Application.Dtos;
using System;
using AuthService.Api.Domain;

namespace AuthService.Api.Controllers
{
    public class AuthManagementController : BaseApiController
    {
        private readonly IUserDataClient _userDataClient;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;

        public AuthManagementController(
            UserManager<AppUser> userManager,
            DataContext dataContext,
            SignInManager<AppUser> signInManager,
            IUserDataClient userDataClient,
            TokenService tokenService)
        {
            _userDataClient = userDataClient;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        // Register User
        [HttpPost("register")]
        public async Task<ActionResult> Registration(UserRegistrationDto user) 
        {
            if (!ModelState.IsValid) return BadRequest("Invalid payload");

            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null) return BadRequest("Email taken");

            if (await _userManager.Users.AnyAsync(x => x.UserName == user.Username)) return BadRequest("Username taken");

            var newUser = new AppUser() { Email = user.Email, UserName = user.Username };

            var isCreated = await _userManager.CreateAsync(newUser, user.Password);
            if (!isCreated.Succeeded)
            {
                return BadRequest(new UserRegistrationResponseDto()
                {
                     Errors = isCreated.Errors.Select(x => x.Description).ToList(),
                     Success = false
                 });
            }

            // generate token using the tokenservice 
            var jwtToken = await _tokenService.CreateToken(newUser);

            //Pass the user information to Profile EndPoint
            try
            {
                 var userData = new UserSendDto() {Id = newUser.Id, Username = newUser.UserName ,Email = newUser.Email};
                 await _userDataClient.SendUserToProfile(userData);
            }
            catch (Exception ex)
            {
               Console.WriteLine($"Could not send synchronously: {ex.Message}");
            }
            return Ok(jwtToken);
        }

        // Login User
        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDto user)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Login request");
            var userLogin = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (user == null) return Unauthorized();

            var isCorrect = await _signInManager.CheckPasswordSignInAsync(userLogin, user.Password, false);

            if (!isCorrect.Succeeded) return BadRequest("Invalid login request");

            var userlogged = new AppUser() {Id= userLogin.Id , Email = userLogin.Email, UserName = userLogin.UserName };
            var jwtToken = await _tokenService.CreateToken(userlogged);
            return Ok(jwtToken);
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult> RefreshToken(TokenRequest tokenRequest)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid payload");

            var result = await _tokenService.VerifyAndCreateToken(tokenRequest);
            if(result == null){
                return BadRequest("Invalid token 1");
            }

            return Ok(result);
        }
    }
}
