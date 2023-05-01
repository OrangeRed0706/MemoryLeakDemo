using System.Text.Json.Serialization;
using Library.Tracing.Model;

namespace MemoryLeakDemo.Contract
{
    public abstract class RetriableTraceMessage : TraceMessage
    {
        public int RetryCount { get; set; }
        [JsonIgnore]
        public bool IncludeRetryCount { get; set; } = true;

        public bool ShouldSerializeRetryCount()
        {
            return IncludeRetryCount;
        }
    }
}
