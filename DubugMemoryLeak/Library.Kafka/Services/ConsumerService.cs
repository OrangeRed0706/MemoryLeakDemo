using Confluent.Kafka;
using Library.Kafka.Interfaces;
using Library.Kafka.Options;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Library.Kafka.Services
{
    internal sealed class ConsumerService : IConsumerService
    {
        private readonly ConsumerOptions _consumerOptions;
        private readonly ConcurrentDictionary<string, object> _lockObjects = new();

        public ConsumerService(IOptions<ConsumerOptions> consumerOptions)
        {
            _consumerOptions = consumerOptions.Value;
        }

        IConsumer<TKey, TValue> IConsumerService.GetConsumer<TKey, TValue>(string consumerKey, IDeserializer<TKey> keyDeserializer, IDeserializer<TValue> valueDeserializer)
        {
            var consumerConfig = _consumerOptions[consumerKey];
            var builder =new ConsumerBuilder<TKey, TValue>(consumerConfig);
            if (keyDeserializer != default)
            {
                builder.SetKeyDeserializer(keyDeserializer);
            }
            if (valueDeserializer != default)
            {
                builder.SetValueDeserializer(valueDeserializer);
            }

            return builder.Build();
        }

        IConsumer<TKey, TValue> IConsumerService.GetConsumer<TKey, TValue>(string consumerKey, string consumerGroupId, IDeserializer<TKey> keyDeserializer, IDeserializer<TValue> valueDeserializer)
        {
            var consumerConfig = _consumerOptions[consumerKey];
            var lockObj = _lockObjects.GetOrAdd(consumerKey, _ => new object());

            lock (lockObj)
            {
                consumerConfig.GroupId = consumerGroupId;

                var builder = new ConsumerBuilder<TKey, TValue>(consumerConfig);
                if (keyDeserializer != default)
                {
                    builder.SetKeyDeserializer(keyDeserializer);
                }
                if (valueDeserializer != default)
                {
                    builder.SetValueDeserializer(valueDeserializer);
                }

                return builder.Build(); 
            }
        }
    }
}
