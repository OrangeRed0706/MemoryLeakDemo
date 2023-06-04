using Confluent.Kafka;

namespace Library.Kafka.Interfaces
{
    public interface IConsumerService
    {
        IConsumer<TKey, TValue> GetConsumer<TKey, TValue>(string consumerKey, IDeserializer<TKey> keyDeserializer = default, IDeserializer<TValue> valueDeserializer = default);

        IConsumer<TKey, TValue> GetConsumer<TKey, TValue>(string consumerKey, string consumerGroupId, IDeserializer<TKey> keyDeserializer =default, IDeserializer<TValue> valueDeserializer =default);
    }
}
