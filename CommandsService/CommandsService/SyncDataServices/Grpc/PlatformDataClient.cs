using CommandsService.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using PlatformService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _config;

        public PlatformDataClient(IConfiguration config)
        {
            _config = config;
        }
        public IEnumerable<Platform> ReturnAllPlatorms()
        {
            var channel = GrpcChannel.ForAddress(_config["GrpcPlatform"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();
            try
            {
                var repy = client.GetAllPlatforms(request);
                return repy.Platform.Select(p => new Platform
                {
                    Name = p.Name,
                    ExternalId = p.PlatformId
                });
            }
            catch (Exception ex)
            {

                Console.WriteLine("fail grpc "+ ex);
                return null;
            }
        }
    }
}
