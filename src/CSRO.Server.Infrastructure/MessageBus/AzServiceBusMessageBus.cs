using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Infrastructure.MessageBus
{
    public class AzServiceBusMessageBus : IMessageBus
    {
        //TODO: read from settings
        private readonly string _connectionString;
        public AzServiceBusMessageBus(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureServiceBus");
        }

        public async Task PublishMessage(BusMessageBase message, string topicName)
        {
            ISenderClient topicClient = new TopicClient(_connectionString, topicName);

            var jsonMessage = JsonConvert.SerializeObject(message);
            var serviceBusMessage = new Message(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await topicClient.SendAsync(serviceBusMessage);
            Console.WriteLine($"Sent message to {topicClient.Path}");
            await topicClient.CloseAsync();
        }
    }
}
