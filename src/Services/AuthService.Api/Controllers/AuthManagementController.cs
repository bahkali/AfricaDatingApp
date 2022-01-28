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
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AuthService.Api.Controllers
{
    public class AuthManagementController : BaseApiController
    {
        private readonly IUserDataClient _userDataClient;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public AuthManagementController(
            UserManager<AppUser> userManager,
            DataContext dataContext,
            IConfiguration config,
            SignInManager<AppUser> signInManager,
            IUserDataClient userDataClient,
            TokenService tokenService)
        {
            _userDataClient = userDataClient;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _config = config;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri("https://graph.facebook.com")
            };
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
            if (userLogin == null) return Unauthorized();

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

        [HttpPost("fbLogin")]
        public async Task<ActionResult> FacebookLogin(string accessToken)
        {
            // check the access token
            var fbVerifyKeys = _config["Favebook:AppId"] + "|" + _config["Facebook:AppSecret"];
            var verifyToken = await _httpClient.GetAsync($"debug_token?input_token={accessToken}&access_token={fbVerifyKeys}");
            if(!verifyToken.IsSuccessStatusCode) return Unauthorized();

            var fbUrl = $"me?access_token={accessToken}&fields=name,email";
            var response = await _httpClient.GetAsync(fbUrl);
            if(!response.IsSuccessStatusCode) return Unauthorized();

            var content = await response.Content.ReadAsStringAsync();
            var fbInfo = JsonConvert.DeserializeObject<dynamic>(content);

            
            var email = (string)fbInfo.name;
            var userExist = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == email);

            if(userExist != null){
                var userlogged = new AppUser() {Id= userExist.Id , Email = userExist.Email, UserName = userExist.UserName };
                var Token = await _tokenService.CreateToken(userlogged);
                return Ok(Token);
            }

            var newUser = new AppUser() 
            { 
                Email = (string)fbInfo.email, 
                UserName = (string)fbInfo.id
            };

            var isCreated = await _userManager.CreateAsync(newUser);
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

            return Ok(jwtToken);
        }
    
    }
}
