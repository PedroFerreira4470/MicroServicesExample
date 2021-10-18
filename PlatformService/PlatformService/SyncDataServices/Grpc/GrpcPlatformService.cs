using Grpc.Core;
using PlatformService.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService: GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlataformRepository _repo;

        public GrpcPlatformService(IPlataformRepository repo)
        {
            _repo = repo;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var response = new PlatformResponse();
            var platforms = _repo.GetAllPlatforms();

            foreach (var pla in platforms)
            {
                response.Platform.Add(new GrpcPlatformModel { 
                     Name = pla.Name,
                     PlatformId = pla.Id,
                     Publisher = pla.Publisher
                });
            }

            return Task.FromResult(response);
        }
    }
}
