using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatoformPublished:
                    AddPlatform(message);
                    break;
                case EventType.Undetermined:
                    break;
                default:
                    break;
            }
        }

        private static EventType DetermineEvent(string notificationMessage) 
        {
            Console.WriteLine("Determining Event");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("Platform_Published event triggered");
                    return EventType.PlatoformPublished;
                default:
                    Console.WriteLine("event not triggered");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMessage) 
        {
            UsingCommandRepository((repo) =>
            {
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                try
                {
                    var plat = new Platform()
                    {
                        ExternalId = platformPublishedDto.Id,
                        Name = platformPublishedDto.Name
                    };
                    if (!repo.ExternalPlatformExists(plat.ExternalId))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("Already Exists");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not add Platform to DB {ex.Message}");
                }
            });
        }

        private void UsingCommandRepository(Action<ICommandRepo> action)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
            action(repo);
        }
    }

    enum EventType 
    {
        PlatoformPublished,
        Undetermined
    }
}
