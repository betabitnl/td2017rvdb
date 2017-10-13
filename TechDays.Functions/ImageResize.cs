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
        #region Fields

        private static Instructions _instructions = new Instructions
        {
            Width = 150,
            Height = 150,
            Mode = FitMode.Crop,
            Scale = ScaleMode.Both
        };

        #endregion

        [FunctionName("ImageResize")]
        public static async Task<object> Run([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
        {
            string jsonContent = await req.Content.ReadAsStringAsync();
            log.Info(jsonContent);

            try
            {
                dynamic request = JsonConvert.DeserializeObject<dynamic>(jsonContent)[0];

                using (var original = new MemoryStream())
                using (var thumb = new MemoryStream())
                {
                    string url = request.data.url;
                    var filename = url.Substring(url.LastIndexOf('/') + 1);
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer inputContainer = blobClient.GetContainerReference("INPUT_CONTAINERNAME");
                    CloudBlockBlob originalBlob = inputContainer.GetBlockBlobReference(filename);
                    CloudBlobContainer outputContainer = blobClient.GetContainerReference("OUTPUT_CONTAINERNAME");
                    CloudBlockBlob blockBlob = outputContainer.GetBlockBlobReference($"thumb-{filename}");

                    await originalBlob.DownloadToStreamAsync(original);
                    original.Seek(0, SeekOrigin.Begin);
                    ImageBuilder.Current.Build(new ImageJob(original, thumb, _instructions));
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