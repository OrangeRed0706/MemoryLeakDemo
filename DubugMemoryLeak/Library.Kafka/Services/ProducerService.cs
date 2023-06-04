using Confluent.Kafka;
using Library.Kafka.Interfaces;
using Library.Kafka.Options;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using ProducerConfig = Confluent.Kafka.ProducerConfig;

namespace Library.Kafka.Services
{
    internal sealed class ProducerService : IProducerService
    {
        private readonly ProducerOptions _producerOptions;
        private static readonly ConcurrentDictionary<string, dynamic> ProducerDict = new();

        public ProducerService(IOptions<ProducerOptions> producerOptions)
        {
            _producerOptions = producerOptions.Value;
        }

        IProducer<TKey, TValue> IProducerService.GetProducer<TKey, TValue>(string producerKey, ISerializer<TKey>? keySerializer, ISerializer<TValue>? valueSerializer)
        {

            return ProducerDict.GetOrAdd($"{typeof(TKey).Name}-{typeof(TValue).Name}-{producerKey}", _ =>
            {
                var config = _producerOptions[producerKey];
                var builder = new ProducerBuilder<TKey, TValue>(config);
                if (keySerializer != default)
                {
                    builder.SetKeySerializer(keySerializer);
                }
                if (valueSerializer != default)
                {
                    builder.SetValueSerializer(valueSerializer);
                }
                return builder.Build();
            });
        }
    }
}
