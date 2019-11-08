using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace erikitofunctions
{
    public static class TimerTrigger
    {
        [FunctionName("TimerTrigger")]
        public static async Task Run([TimerTrigger("0 */15 * * * *")]TimerInfo timer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            log.LogInformation($"Timer schedule: {timer.Schedule}");
            log.LogInformation($"Timer last execution: {timer.ScheduleStatus.Last}");
            log.LogInformation($"Timer last execution: {timer.ScheduleStatus.Next}");

            string connectionstring = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            CloudStorageAccount account = CloudStorageAccount.Parse(connectionstring);
            CloudBlobClient client = account.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference("sms-receipts");

            DateTime oldestAllowedTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(1));

            BlobContinuationToken token = null;

            do
            {
                var response = await container.ListBlobsSegmentedAsync(token);
                token = response.ContinuationToken;

                foreach (var blob in response.Results.OfType<CloudBlockBlob>())
                {
                    bool tooOld = blob.Properties.LastModified < oldestAllowedTime;

                    if (tooOld)
                    {
                        log.LogInformation($"Deleting blob {blob.Name}");
                        await blob.DeleteAsync();
                    }
                }
            } while (token != null);
        }
    }
}
