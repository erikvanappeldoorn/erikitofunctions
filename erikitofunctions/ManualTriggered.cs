using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace erikitofunctions
{
    public static class ManualTrigger
    {
        [NoAutomaticTrigger]
        [FunctionName("ManualTrigger")]
        [StorageAccount("AzureWebJobsStorage")]
        [return: Queue("greeting-creation-requests")]
        public static CreateGreetingRequest Run(string input, 
                               ILogger log)
        {
            log.LogInformation($"This is a GitHub manually triggered C# function with input {input}");
            return new CreateGreetingRequest { FirstName = input, Number = "1234567890" };
        }
    }
}
