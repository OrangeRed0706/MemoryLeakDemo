using Confluent.Kafka;

namespace Library.Kafka.Interfaces
{
    public interface IProducerService
    {
        IProducer<TKey, TValue> GetProducer<TKey, TValue>(string producerKey = default, ISerializer<TKey>? keySerializer = default, ISerializer<TValue>? valueSerializer = default);
    }
}
