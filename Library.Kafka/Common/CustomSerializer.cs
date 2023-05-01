using Confluent.Kafka;
using System.Text;
using System.Text.Json;
using Library.Kafka.Interfaces;

namespace Library.Kafka.Common
{
    public class CustomSerializer<T> : ICustomSerializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context) =>
            JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data.ToArray()))!;

        public byte[] Serialize(T data, SerializationContext context) =>
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
    }
}
