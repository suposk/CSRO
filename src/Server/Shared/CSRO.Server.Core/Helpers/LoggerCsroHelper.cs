using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerCsroHelper
    {
        public static void LogErrorCsro(this ILogger logger, Exception exception, [CallerMemberName] string memberName = null)
        {
            logger.LogError(exception, $"CSRO Error => {memberName}");
        }
    }
}