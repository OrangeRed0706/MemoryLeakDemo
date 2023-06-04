using System.Collections.Concurrent;

namespace Library.Kafka.Options
{
    public class ConsumerOptions : ConcurrentDictionary<string, Confluent.Kafka.ConsumerConfig>
    {
    }
}
