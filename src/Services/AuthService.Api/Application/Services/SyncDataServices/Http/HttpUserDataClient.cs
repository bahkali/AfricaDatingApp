using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AuthService.Api.Application.Dtos;
using Microsoft.Extensions.Configuration;

namespace AuthService.Api.Application.Services.SyncDataServices.Http
{
    public class HttpUserDataClient : IUserDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public HttpUserDataClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }
        public async Task SendUserToProfile(UserSendDto user)
        {
            // First Serialize the payload
            var httpContent = new StringContent(
                JsonSerializer.Serialize(user),
                Encoding.UTF8,
                "application/json"
            );

            // make the post request
            var response = await _httpClient.PostAsync(_config["ProfileServiceEndPoint"], httpContent);
            if(response.IsSuccessStatusCode){
                Console.WriteLine("sync Post to ProfileService was ok!");
            }else{
                Console.WriteLine("Sync Post to ProfileService was not ok!");
            }
        }
    }
}