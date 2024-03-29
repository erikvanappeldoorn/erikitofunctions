using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace erikitofunctions
{
    public static class QueueTrigger
    {
        [FunctionName("QueueTrigger")]
        [StorageAccount("AzureWebJobsStorage")]
        [return: Blob("greeting-requests/{rand-guid}")]
        public static string Run([QueueTrigger("greeting-creation-requests")]CreateGreetingRequest input, 
                                 ILogger log,
                                 DateTimeOffset insertionTime,
                                 string id)
        {
            log.LogInformation($"C# Queue trigger function processed: {input}");
            log.LogInformation($"Insertion time: {insertionTime}");
            log.LogInformation($"Id: {id}");

            var greetingRequest = new GreetingRequest
            {
                Number = input.Number,
                Message = $"Hi {input.FirstName}"
            };

            return JsonConvert.SerializeObject(greetingRequest);
        }
    }
}
