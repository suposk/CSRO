using System.Threading.Tasks;

namespace CSRO.Server.Infrastructure.MessageBus
{
    public interface IServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
