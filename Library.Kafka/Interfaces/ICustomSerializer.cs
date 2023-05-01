using Confluent.Kafka;

namespace Library.Kafka.Interfaces
{
    public interface ICustomSerializer<T> : ISerializer<T>, IDeserializer<T>
    {

    }
}
