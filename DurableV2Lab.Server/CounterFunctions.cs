using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace DurableV2Lab.Server
{
    public static class CounterFunctions
    {
        [FunctionName("Increment")]
        public static async Task<IActionResult> Increment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "counter/{id}")] HttpRequest req,
            string id,
            [DurableClient]IDurableEntityClient client,
            ILogger log)
        {
            if (!int.TryParse(await req.ReadAsStringAsync(), out var amount))
            {
                return new BadRequestResult();
            }

            await client.SignalEntityAsync<ICounterEntity>(id, x => x.IncrementAsync(amount));
            return new OkResult();
        }

        [FunctionName("Get")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "counter/{id}")] HttpRequest req,
            string id,
            [DurableClient]IDurableEntityClient client,
            ILogger log)
        {
            return new OkObjectResult(await client.ReadEntityStateAsync<CounterEntity>(new EntityId(nameof(CounterEntity), id)));
        }
    }
}
