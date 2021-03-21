using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Infrastructure.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessage(BusMessageBase message, string topicName);
    }
}
