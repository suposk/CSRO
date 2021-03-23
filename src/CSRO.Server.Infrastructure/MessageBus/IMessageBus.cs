using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Infrastructure.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessageTopic(BusMessageBase message, string topicName);
        Task PublishMessageQueue(BusMessageBase message, string queueName);
    }
}
