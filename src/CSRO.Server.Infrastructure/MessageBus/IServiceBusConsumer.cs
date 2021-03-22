namespace CSRO.Server.Infrastructure.MessageBus
{
    public interface IServiceBusConsumer
    {
        void Start();
        void Stop();
    }
}
