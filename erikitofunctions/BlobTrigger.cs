using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace erikitofunctions
{
    public static class BlobTrigger
    {
        [FunctionName("BlobTrigger")]
        [StorageAccount("AzureWebJobsStorage")]
        [return: Blob("sms-receipts/{rand-guid}")]
        public static async Task<string> Run([BlobTrigger("greeting-requests/{name}")]CloudBlockBlob blob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name}");
            log.LogInformation("Metadata");
            log.LogInformation($"- Name: {blob.Name}");
            log.LogInformation($"- StorageUri: {blob.StorageUri}");
            log.LogInformation($"- Container: {blob.Container.Name}");

            var greetingRequest = await DownloadGreetingRequest(blob);
            log.LogInformation($"- Downloaded: {greetingRequest}");
            
            string sendReceiptId = SendSms(greetingRequest);

            var confirmation = new SmsSentConfirmation
            {
                ReceiptId = sendReceiptId,
                Message = greetingRequest.Message,
                Number = greetingRequest.Number
            };

            return JsonConvert.SerializeObject(confirmation);

        }

        public static async Task<GreetingRequest> DownloadGreetingRequest(CloudBlockBlob blob)
        {
            GreetingRequest greetingRequest;

            using (var ms = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(ms);
                ms.Position = 0;

                using (var sr = new StreamReader(ms))
                {
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        var serializer = new JsonSerializer();
                        greetingRequest = serializer.Deserialize<GreetingRequest>(jsonTextReader);
                    }
                }
            }
            return greetingRequest;
        }

        public static string SendSms(GreetingRequest request)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
