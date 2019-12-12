using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DurableV2Lab.Server
{
    public interface ICounterEntity
    {
        Task IncrementAsync(int amount);
        Task DecrementAsync(int amount);
        Task<int> GetAsync();
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CounterEntity : ICounterEntity
    {
        [JsonProperty("value")]
        public int Value { get; set; }

        [FunctionName(nameof(CounterEntity))]
        public static Task Run([EntityTrigger]IDurableEntityContext context)
        {
            return context.DispatchAsync<CounterEntity>();
        }

        public Task IncrementAsync(int amount)
        {
            Value += amount;
            return Task.CompletedTask;
        }

        public Task DecrementAsync(int amount)
        {
            Value -= amount;
            return Task.CompletedTask;
        }

        public Task<int> GetAsync() => Task.FromResult(Value);
    }
}
