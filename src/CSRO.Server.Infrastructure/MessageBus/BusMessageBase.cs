using System;

namespace CSRO.Server.Infrastructure.MessageBus
{
    public class BusMessageBase
    {
        public BusMessageBase()
        {
            Id = Guid.NewGuid();
            CreationDateTimeUtc = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public DateTime CreationDateTimeUtc { get; set; }
    }
}
