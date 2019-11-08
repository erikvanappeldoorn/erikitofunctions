using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace erikitofunctions
{
    public static class HttpTriggerGet
    {
        [FunctionName("HttpTriggerGet")]
        [StorageAccount("AzureWebJobsStorage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            if (name == null)
            {
                return new NotFoundResult();
            }

            log.LogInformation($"Blob to load {name}");
            SmsSentConfirmation confirmation = await DownloadSmsSentConfirmation(name);

            if (confirmation == null)
            {
                return new NotFoundResult();
            }

            log.LogInformation($"Downloaded: {confirmation}");

            return new OkObjectResult(confirmation);
        }

        public static async Task<SmsSentConfirmation> DownloadSmsSentConfirmation(string name)
        {
            string connectionstring = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            CloudStorageAccount account = CloudStorageAccount.Parse(connectionstring);
            CloudBlobClient client = account.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference("sms-receipts");

            CloudBlockBlob smsReceiptBlob = container.GetBlockBlobReference(name);

            if (!await smsReceiptBlob.ExistsAsync())
            {
                return null;
            }

            SmsSentConfirmation confirmation;

            using (var ms = new MemoryStream())
            {
                await smsReceiptBlob.DownloadToStreamAsync(ms);
                ms.Position = 0;

                using (var sr = new StreamReader(ms))
                {
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        var serializer = new JsonSerializer();
                        confirmation = serializer.Deserialize<SmsSentConfirmation>(jsonTextReader);
                    }
                }
            }
            return confirmation;
        }
    }
}
