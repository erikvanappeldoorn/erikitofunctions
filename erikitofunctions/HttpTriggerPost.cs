using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace erikitofunctions
{
    public static class HttpTriggerPost
    {
        [FunctionName("HttpTriggerPost")]
        [StorageAccount("AzureWebJobsStorage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req,
            ILogger log,
            [Queue("greeting-creation-requests")]IAsyncCollector<CreateGreetingRequest> outputQueue)
        {
            string jsonContent = await req.ReadAsStringAsync();
            CreateGreetingRequest data = JsonConvert.DeserializeObject<CreateGreetingRequest>(jsonContent);
            await outputQueue.AddAsync(data);

            log.LogInformation($"Added {data} to queue");

            return new OkResult();
        }
    }
}
