using Confluent.Kafka;
using System.Collections.Concurrent;

namespace Library.Kafka.Options
{
    public class ProducerOptions : ConcurrentDictionary<string, Confluent.Kafka.ProducerConfig>
    {

    }
}
