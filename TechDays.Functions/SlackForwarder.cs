using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;

namespace TechDays.Functions
{
    public static class SlackForwarder
    {
        #region Constants

        private const string MESSAGE = "{\"text\": \"*HEADS UP* There's a new file uploaded.\", \"username\": \"Rick @ TD2017\", \"icon_url\": \"https://pbs.twimg.com/media/DJsk5nkXgAEcG5x.jpg\"}";

        #endregion

        [FunctionName("SlackForwarder")]
        public static async Task<object> Run([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                string jsonContent = await req.Content.ReadAsStringAsync();
                var evnt = JsonConvert.DeserializeObject<List<EventGridEvent>>(jsonContent).First();

                string response;
                using (var client = new WebClient())
                {
                    var url = evnt.Data["url"].ToString();
                    log.Info(url);
                    string filename = url.Substring(url.LastIndexOf('/') + 1);
                    log.Info(filename);
                    response = client.UploadString("https://hooks.slack.com/services/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", 
                        MESSAGE);
                    log.Info(response);
                }

                return req.CreateResponse(HttpStatusCode.OK, new
                {
                    status = "All done!",
                    result = response
                });
            }
            catch (Exception ex)
            {
                log.Warning(ex.Message);
                if (ex.InnerException != null)
                {
                    log.Warning(ex.InnerException.Message);
                }
                return null;
            }
        }
    }
}