using Confluent.Kafka;
using Library.Kafka.Interfaces;
using Library.Tracing.Interface;
using MemoryLeakDemo.Contract;
using MemoryLeakDemo.Extension;
using Microsoft.AspNetCore.Mvc;

namespace MemoryLeakDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProduceMessageController : ControllerBase
    {
        private readonly ITraceManager _traceManager;
        private readonly IProducerService _producerService;
        private readonly ICustomSerializer<GenerateMessage>? _messageSerializer;

        public ProduceMessageController(ILogger<ProduceMessageController> logger,
            ITraceManager traceManager,
            IProducerService producerService,
            ICustomSerializer<GenerateMessage>? messageSerializer
            )
        {
            _traceManager = traceManager;
            _producerService = producerService;
            _messageSerializer = messageSerializer;
        }

        [HttpPost(Name = "produce-message")]
        public async Task ProduceMessage([FromQuery] int count, CancellationToken cancellationToken)
        {
            var taskList = new List<Task>();
            var i = 0;
            for (i = 0; i < count; i++)
            {
                taskList.Add(Produce(i, cancellationToken));
            }

            await Task.WhenAll(taskList).ConfigureAwait(false);
        }

        private async Task Produce(int id, CancellationToken cancellationToken)
        {
            var producer = _producerService.GetProducer<string, GenerateMessage>("MessageGenerate", valueSerializer: _messageSerializer);

            var message = new GenerateMessage
            {
                Id = Guid.NewGuid(),
                Value = id,
                RetryCount = 0,
                CreatedDate = DateTimeOffset.UtcNow.ToLocalTime(),
            };

            await producer.ProduceAsync("my-topic", new Message<string, GenerateMessage>
            {
                Key = DateTimeOffset.UtcNow.ToLocalTime().ToString(),
                Value = message
            }, _traceManager, cancellationToken).ConfigureAwait(false);
        }
    }
}