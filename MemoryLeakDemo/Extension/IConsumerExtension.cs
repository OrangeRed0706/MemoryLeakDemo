using Confluent.Kafka;
using Jaeger;
using Library.Tracing.Interface;
using Library.Tracing.Model;
using OpenTracing;
using OpenTracing.Tag;

namespace MemoryLeakDemo.Extension
{
    public static class IConsumerExtension
    {
        public static ConsumeResult<TKey, TValue> Consume<TKey, TValue>(
            this IConsumer<TKey, TValue> consumer,
            out IScope scope,
            ITraceManager traceManager,
            string? tracerName = default,
            CancellationToken cancellationToken = default) where TValue : TraceMessage
        {
            var topic = consumer.Subscription.FirstOrDefault();
            var consumeResult = consumer.Consume(cancellationToken);
            if (consumeResult.IsPartitionEOF)
            {
                scope = traceManager.StartScopeTrace(topic, b => b.IgnoreActiveSpan());
                return consumeResult;
            }

            if (consumeResult.Message.Value.SpanContext != default)
            {
                var context = SpanContext.ContextFromString(consumeResult.Message.Value.SpanContext);
                ISpan? span = null;
                using (var parentScope = traceManager.CreateSpanBuilder(topic)
                           .IgnoreActiveSpan()
                           .AsChildOf(context)
                           .WithTag(Tags.Component, $"Consumer:{topic}")
                           .WithTag(Tags.SpanKind, Tags.SpanKindServer)
                           .WithTag("kafka.type", "kafka.consume")
                           .WithTag("kafka.topic", consumeResult.Topic)
                           .WithTag("kafka.offset", consumeResult.Offset)
                           .WithTag("kafka.partition", consumeResult.Partition)
                           .StartActive(true))
                {

                    span = parentScope.Span;
                    //scope = traceManager.CreateSpanBuilder(tracerName ?? $"Consumer:{tracerName}")
                    //    .AsChildOf(parentScope.Span)
                    //    .StartActive(true);
                    //parentScope.Span.Finish();
                }
                scope = traceManager.CreateSpanBuilder(tracerName ?? $"Consumer:{tracerName}")
                    .AsChildOf(span)
                    .StartActive(true);
            }
            else
            {
                scope = traceManager.StartScopeTrace(topic, b => b.IgnoreActiveSpan());
            }
            return consumeResult;
        }

        public static ConsumeResult<TKey, TValue> MemoryLeakConsume<TKey, TValue>(
            this IConsumer<TKey, TValue> consumer,
            out IScope scope,
            ITraceManager traceManager,
            string? tracerName = default,
            CancellationToken cancellationToken = default) where TValue : TraceMessage
        {
            var topic = consumer.Subscription.FirstOrDefault();
            var consumeResult = consumer.Consume(cancellationToken);
            if (consumeResult.IsPartitionEOF)
            {
                scope = traceManager.StartScopeTrace(topic, b => b.IgnoreActiveSpan());
                return consumeResult;
            }

            if (consumeResult.Message.Value.SpanContext != default)
            {
                var context = SpanContext.ContextFromString(consumeResult.Message.Value.SpanContext);
                using (var parentScope = traceManager.CreateSpanBuilder(topic)
                           .IgnoreActiveSpan()
                           .AsChildOf(context)
                           .WithTag(Tags.Component, $"Consumer:{topic}")
                           .WithTag(Tags.SpanKind, Tags.SpanKindServer)
                           .WithTag("kafka.type", "kafka.consume")
                           .WithTag("kafka.topic", consumeResult.Topic)
                           .WithTag("kafka.offset", consumeResult.Offset)
                           .WithTag("kafka.partition", consumeResult.Partition)
                           .StartActive(true))
                {
                    scope = traceManager.CreateSpanBuilder(tracerName ?? $"Consumer:{tracerName}")
                        .AsChildOf(parentScope.Span)
                        .StartActive(true);
                }
            }
            else
            {
                scope = traceManager.StartScopeTrace(topic, b => b.IgnoreActiveSpan());
            }
            return consumeResult;
        }
    }
}
