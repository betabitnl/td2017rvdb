using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using ImageResizer;
using Newtonsoft.Json;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TechDays.Functions
{
    public static class ImageResize
    {
        [FunctionName("ImageResize")]
        public static async Task<object> Run([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
        {
            string jsonContent = await req.Content.ReadAsStringAsync();
            log.Info(jsonContent);

            try
            {
                dynamic request = JsonConvert.DeserializeObject<dynamic>(jsonContent)[0];

                var instructions = new Instructions
                {
                    Width = 150,
                    Height = 150,
                    Mode = FitMode.Crop,
                    Scale = ScaleMode.Both
                };

                using (var original = new MemoryStream())
                using (var thumb = new MemoryStream())
                {
                    string url = request.data.url;
                    var filename = url.Substring(url.LastIndexOf('/') + 1);
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=td2017gs2storage;AccountKey=4aVNGE9TeT4O1GwsM653pyGKsxgN2PfIRepJAomzweDvMn16ussrRV+Y7yLa+/+6YyeAZvEujKK0QZei9yCOmg==;EndpointSuffix=core.windows.net");
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer inputContainer = blobClient.GetContainerReference("input");
                    CloudBlockBlob originalBlob = inputContainer.GetBlockBlobReference(filename);
                    CloudBlobContainer outputContainer = blobClient.GetContainerReference("processed");
                    CloudBlockBlob blockBlob = outputContainer.GetBlockBlobReference($"thumb-{filename}");

                    await originalBlob.DownloadToStreamAsync(original);
                    original.Seek(0, SeekOrigin.Begin);
                    ImageBuilder.Current.Build(new ImageJob(original, thumb, instructions));
                    thumb.Seek(0, SeekOrigin.Begin);

                    await blockBlob.UploadFromStreamAsync(thumb);
                }

                log.Info("Aaaaaaand we're done.");
                return req.CreateResponse(HttpStatusCode.OK, new
                {
                    result = $"We're all done processing {request.data.url}!"
                });
            }
            catch (Exception e)
            {
                log.Warning(e.Message);
                return req.CreateResponse(HttpStatusCode.BadRequest, e);
            }
        }
    }
}