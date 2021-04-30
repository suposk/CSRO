using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerHelper
    {
        public static void LogSecretVariableValueStartValue(this ILogger logger, string variable, string value)
        {
            const int lengthToLog = 6;
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    //Console.WriteLine($"{nameof(LogSecretVariableValueStartValue)}Console Error->{variable} is null");
                    logger.LogError($"{nameof(LogSecretVariableValueStartValue)}->{variable} is null");
                }
                else
                {
                    //Console.WriteLine($"{nameof(LogSecretVariableValueStartValue)}Console->{variable} = {value.Substring(startIndex: 0, length: lengthToLog)}");
                    logger.LogWarning($"{nameof(LogSecretVariableValueStartValue)}->{variable} = {value.Substring(startIndex: 0, length: lengthToLog)}");
                }
            }
            catch (Exception ex)
            {
                logger?.LogError($"CSRO exception in {nameof(LogSecretVariableValueStartValue)}", ex);
            }
        }
    }
}
