using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace erikitofunctions
{
    public static class SharedCode
    {
        [NoAutomaticTrigger]
        [FunctionName("SharedCode")]
        public static void Run(string input, ILogger log)
        {
            log.LogInformation($"This is a manually triggered C# function with input {input}");
        }
    }
}
