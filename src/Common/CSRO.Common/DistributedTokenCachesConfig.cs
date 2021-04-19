namespace CSRO.Common
{
    public class DistributedTokenCachesConfig
    {
        public bool IsEnabled { get; set; }
        public int DefaultSlidingExpirationMinutes { get; set; }
    }
}
