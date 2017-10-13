using System.Net;
using System.Net.Http;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;


namespace TechDays.Functions
{
    public static class HelloWorld
    {
        [FunctionName("HelloWorld")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post",
            Route = "HelloWorldFunction/name/{name}")]HttpRequestMessage req, string name, TraceWriter log)
        {
            log.Info($"C# HTTP trigger function 'HelloWorld' has processed a request for {name}.");

            return req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        }
    }
}