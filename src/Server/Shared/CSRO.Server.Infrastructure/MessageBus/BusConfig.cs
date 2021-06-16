using System;

namespace CSRO.Server.Infrastructure.MessageBus
{
    public enum BusTypeEnum 
    {
        /// <summary>
        /// Not value read
        /// </summary>
        None, 
        /// <summary>
        /// Azure 
        /// </summary>
        AzureServiceBus, 
        /// <summary>
        /// In memory local dev
        /// </summary>
        InMemoryMassTransit
    }

    /// <summary>
    /// Global Bus config, Disabled bus and type of buss Azure, In memory
    /// </summary>
    public class BusConfig
    {
        public bool IsBusEnabled { get; set; }
        public string BusType { get; set; }
        public int BusDelayStartInSec { get; set; }
        public BusTypeEnum BusTypeEnum => Enum.TryParse(BusType, true, out BusTypeEnum busTypeEnum) ? busTypeEnum : default(BusTypeEnum);

        public override string ToString()
        {
            return $"{nameof(IsBusEnabled)}: {IsBusEnabled}, {nameof(BusType)}: {BusType}, {nameof(BusDelayStartInSec)}: {BusDelayStartInSec}";
        }
    }
}
