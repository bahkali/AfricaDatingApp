using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using AuthService.Api.Domain.Entities;
using AuthService.Api.Domain;
using System.Threading.Tasks;
using AuthService.Api.Application.configurations;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using AuthService.Api.Application.Dtos.Requests;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Application.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly DataContext _bdContext;

        private readonly UserManager<AppUser> _userManager;
        private readonly TokenValidationParameters _tokenValidationParams;
        public TokenService(
            IConfiguration config,
            DataContext bdContext,
            UserManager<AppUser> userManager,
            TokenValidationParameters tokenValidationParameters)
        {
            _bdContext = bdContext;
            _config = config;
            _userManager = userManager;
            _tokenValidationParams = tokenValidationParameters;
        }
        public async Task<AuthResult> CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["JwtConfig:TokenKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(2),
                SigningCredentials = creds,
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevorked = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = RandomString(35) + Guid.NewGuid()
            };

            // Add the refreshtoken in Memory
            await _bdContext.RefreshTokens.AddAsync(refreshToken);
            await _bdContext.SaveChangesAsync();

            return new AuthResult{
                Token = jwtToken,
                Success = true,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<AuthResult> VerifyAndCreateToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // Validate JWT token Format
                 var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParams, out var validatedToken);
                 // Validate encryption alg
                 if(validatedToken is JwtSecurityToken jwtSecurityToken){
                     var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                     if(result == false){
                         return null;
                     }
                 }
                 //Check the expiration date of the token
                 var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                 
                 var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
                 if(expiryDate > DateTime.UtcNow){
                     return new AuthResult(){
                         Success = false,
                         Errors = new List<string>(){
                             "Token has not yet expired"
                         }
                     };
                 }

                 // validate if it exists , is used and revoked
                 var storedToken = await _bdContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);
                 if(storedToken == null){
                     return new AuthResult(){
                         Success = false,
                         Errors = new List<string>(){
                             "Token does not exist"
                         }
                     };
                 }
                 if(storedToken.IsUsed){
                     return new AuthResult(){
                         Success = false,
                         Errors = new List<string>(){
                             "Token has been used"
                         }
                     };
                 }
                 if(storedToken.IsRevorked){
                     return new AuthResult(){
                         Success = false,
                         Errors = new List<string>(){
                             "Token has been revoked"
                         }
                     };
                 }
                 
                 // Validate the token Id 
                 var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                 if(storedToken.JwtId != jti){
                     return new AuthResult(){
                         Success = false,
                         Errors = new List<string>(){
                             "Token doesn't match"
                         }
                    };
                 }

                 // update current Token
                 storedToken.IsUsed = true;
                 _bdContext.RefreshTokens.Update(storedToken);
                 await _bdContext.SaveChangesAsync();
                 // Create a new token
                 var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                 return await CreateToken(dbUser);
            }
            catch (Exception)
            {
                return null;
            }
        }


        private DateTime UnixTimeStampToDateTime(long utcExpiryDate)
        {
            var dateTimeVal = new DateTime(1970, 1,1,0,0,0,0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(utcExpiryDate).ToLocalTime();
            return dateTimeVal;
        }

        // Generate random character
        private string RandomString(int length) 
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(x => x[random.Next(x.Length)]).ToArray());
        }
    }
}