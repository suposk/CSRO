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

        public async Task PublishMessageQueue(BusMessageBase message, string queueName)
        {
            if (message is null)            
                throw new ArgumentNullException(nameof(message));            

            if (string.IsNullOrWhiteSpace(queueName))            
                throw new ArgumentException($"'{nameof(queueName)}' cannot be null or whitespace.", nameof(queueName));
            
            var client = new QueueClient(_connectionString, queueName);
            var jsonMessage = JsonConvert.SerializeObject(message);
            var serviceBusMessage = new Message(Encoding.UTF8.GetBytes(jsonMessage))
            {
                //CorrelationId = Guid.NewGuid().ToString()
            };
            await client.SendAsync(serviceBusMessage);
            Console.WriteLine($"Sent message to Queue {client.Path}");
            await client.CloseAsync();
        }

        public async Task PublishMessageTopic(BusMessageBase message, string topicName)
        {
            if (message is null)            
                throw new ArgumentNullException(nameof(message));            

            if (string.IsNullOrWhiteSpace(topicName))            
                throw new ArgumentException($"'{nameof(topicName)}' cannot be null or whitespace.", nameof(topicName));
            
            ISenderClient client = new TopicClient(_connectionString, topicName);

            var jsonMessage = JsonConvert.SerializeObject(message);
            var serviceBusMessage = new Message(Encoding.UTF8.GetBytes(jsonMessage))
            {
                //CorrelationId = Guid.NewGuid().ToString()
            };
            await client.SendAsync(serviceBusMessage);
            Console.WriteLine($"Sent message to Topic {client.Path}");
            await client.CloseAsync();
        }
    }
}
