using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Core.Helpers
{
    public static class PollyHelper
    {
        public const int DEFAULT_Retry_Count = 1;

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount = DEFAULT_Retry_Count)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                //.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicyJitter(int retryCount = DEFAULT_Retry_Count)
        {
            Random jitterer = new Random();
            var retryWithJitterPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                //.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(retryCount,    // exponential back-off plus some jitter
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                  + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)));
            return retryWithJitterPolicy;
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int retryCount = DEFAULT_Retry_Count)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(retryCount, TimeSpan.FromMinutes(2));
        }
    }
}
