using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.DataAccess;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlataformRepository _repo;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlataformRepository repo,
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient)
        {
            _repo = repo;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public IActionResult GetPlatforms() 
        {
            var items = _repo.GetAllPlatforms();
            var result = items.Select(p => new PlatformReadDto
            {
                Id = p.Id,
                Cost = p.Cost,
                Name = p.Name,
                Publisher = p.Publisher
            });
            return Ok(result);
        }

        [HttpGet("{id}",Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var item = _repo.GetById(id);
            if (item is null)
            {
                NotFound();
            }
            var result = new PlatformReadDto
            {
                Id = item.Id,
                Cost = item.Cost,
                Name = item.Name,
                Publisher = item.Publisher
            };
            return Ok(result);
        }

        [HttpPost]
        public IActionResult CreatePlatform(PlatformCreateDto dto)
        {
            var plat = new Platform
            {
                Cost = dto.Cost,
                Name = dto.Name,
                Publisher = dto.Publisher
            };

            _repo.Create(plat);
            _repo.SaveChanges();


            var result = new PlatformPublishedDto
            {
                Id = plat.Id,
                Name = plat.Name,
            };

            try
            {
                result.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(result);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"could send async: {ex.Message}");
            }


            return CreatedAtRoute(nameof(GetById), new { id = result.Id }, result);
        }

    }
}
