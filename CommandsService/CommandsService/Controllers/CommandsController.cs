using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repository;

        public CommandsController(ICommandRepo repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");

            if (!_repository.PlaformExits(platformId))
            {
                return NotFound();
            }

            var commands = _repository.GetCommandsForPlatform(platformId);

            var result = commands.Select(p => new CommandReadDto
            {
                Id = p.Id,
                CommandLine = p.CommandLine,
                HowTo = p.HowTo,
                PlatformId = p.PlatformId
            });
            return Ok(result);
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");

            if (!_repository.PlaformExits(platformId))
            {
                return NotFound();
            }

            var command = _repository.GetCommand(platformId, commandId);

            if (command == null)
            {
                return NotFound();
            }

            var result =  new CommandReadDto
            {
                Id = command.Id,
                CommandLine = command.CommandLine,
                HowTo = command.HowTo,
                PlatformId = command.PlatformId
            };
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

            if (!_repository.PlaformExits(platformId))
            {
                return NotFound();
            }

            var command = new Command
            {
                CommandLine = commandDto.CommandLine,
                HowTo = commandDto.HowTo,
            };

            _repository.CreateCommand(platformId, command);
            _repository.SaveChanges();

            var commandReadDto = new CommandReadDto
            {
                Id = command.Id,
                CommandLine = command.CommandLine,
                HowTo = command.HowTo,
                PlatformId = command.PlatformId
            };

            return CreatedAtRoute(nameof(GetCommandForPlatform),
                new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
        }
    }
}
