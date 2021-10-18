using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration config)
        {
            _config = config;
            var factory = new ConnectionFactory()
            {
                HostName = _config["RabbitMQHost"],
                Port = int.Parse(_config["RabbitMQPort"])

            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare("trigger",ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;
            }
            catch (Exception)
            {

                Console.WriteLine("error rabbitmt factory");
            }

        }

        public void PublishNewPlatform(PlatformPublishedDto dto)
        {
            var message = JsonSerializer.Serialize(dto);
            if (_connection.IsOpen)
            {
                SendMessage(message);
            }

        }

        private void SendMessage(string message) 
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish("trigger","",null, body);
            Console.WriteLine("we sent " + message);
        }


        public void Dispose() 
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Dispose();
            }
        }

        private void RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("shutdown");
        }

    }
}
