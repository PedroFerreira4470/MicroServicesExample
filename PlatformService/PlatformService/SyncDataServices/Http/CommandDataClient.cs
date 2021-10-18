using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlatformService.SyncDataServices.Http
{
    public class CommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CommandDataClient(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(plat),
                Encoding.UTF8,
                "application/json");

            var response 
                = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);
            //api/c/platorms/
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Sync POST to Command Service was ok");
            }
            else
            {
                Console.WriteLine("Sync POST to Command Service was not ok");
            }
        }
    }
}
