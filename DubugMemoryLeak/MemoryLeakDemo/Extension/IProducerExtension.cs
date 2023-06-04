using Confluent.Kafka;
using Library.Tracing.Interface;
using Library.Tracing.Model;
using OpenTracing.Tag;

namespace MemoryLeakDemo.Extension
{
    public static class IProducerExtension
    {
        public static Task<DeliveryResult<TKey, TValue>> ProduceAsync<TKey, TValue>(
            this IProducer<TKey, TValue> producer,
            string topic,
            Message<TKey, TValue> message,
            ITraceManager traceManager,
            CancellationToken cancellationToken = default) where TValue : TraceMessage
        {
            var span = traceManager.CreateSpanBuilder(topic)
                .WithTag(Tags.Component, $"Producer:{topic}")
                .WithTag(Tags.SpanKind, Tags.SpanKindServer)
                .WithTag("kafka.type", "kafka.produce")
                .WithTag("kafka.topic", topic)
                .Start();

            message.Value.SpanContext = span.Context.ToString();
            span.Finish();
            return producer.ProduceAsync(topic, message, cancellationToken: cancellationToken);
        }
    }
}
