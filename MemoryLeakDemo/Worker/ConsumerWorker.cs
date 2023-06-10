using Confluent.Kafka;
using Library.Kafka.Interfaces;
using Library.Tracing.Interface;
using MemoryLeakDemo.Contract;
using MemoryLeakDemo.Extension;
using System.Text.Json;

namespace MemoryLeakDemo.Worker
{
    public class ConsumerWorker : BackgroundService
    {
        public const string Topic = "my-topic";
        private readonly ILogger<ConsumerWorker> _logger;
        private readonly ITraceManager _traceManager;
        private readonly IConsumerService _consumerService;
        private readonly ICustomSerializer<GenerateMessage> _messageSerializer;

        public ConsumerWorker(ILogger<ConsumerWorker> logger,
            ITraceManager traceManager,
            IConsumerService consumerService,
            ICustomSerializer<GenerateMessage> messageSerializer)
        {
            _logger = logger;
            _traceManager = traceManager;
            _consumerService = consumerService;
            _messageSerializer = messageSerializer;
        }

        private async Task ConsumerAsync(int numberOfConsumers, CancellationToken cancellationToken)
        {
            for (var i = 1; i < numberOfConsumers; i++)
            {
                var groupId = $"ConsumerGroup:[{i}]";
                Task.Factory.StartNew(() => RunConsumerAsync(groupId, cancellationToken), TaskCreationOptions.LongRunning);
            }
        }

        private async Task RunConsumerAsync(string consumerGroupId, CancellationToken cancellationToken)
        {
            var consumer = _consumerService.GetConsumer<string, GenerateMessage>("MessageConsumer", consumerGroupId, valueDeserializer: _messageSerializer);
            consumer.Subscribe(Topic);
            Console.WriteLine(consumer.Name);

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Wager Job is going to stop due to cancel command");
                    break;
                }
                try
                {
                    var consumeResult = consumer.MemoryLeakConsume(out var trace, _traceManager, "ConsumerWorker", cancellationToken);
                    using (trace)
                    {
                        if (consumeResult.IsPartitionEOF)
                        {
                            _logger.LogInformation($"{consumerGroupId} Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");
                            continue;
                        }

                        _logger.LogInformation($"{nameof(ConsumerWorker)} {consumerGroupId} ,partition:{consumeResult.Partition} offset: {consumeResult.Offset} Topic:my-topic, Receive message: {JsonSerializer.Serialize(consumeResult.Message.Value)}");

                        try
                        {
                            consumer.Commit(consumeResult);
                        }
                        catch (KafkaException e)
                        {
                            _logger.LogError(e, e.Message);
                        }

                    }
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, $"Consume error: {e.Error.Reason}");
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogError(ex, "task was canceled");
                }
                catch (OperationCanceledException e)
                {
                    _logger.LogWarning(e, "The operation was canceled");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"stop job due to encounter error message:{e.Message}");
                }
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Job is starting.");

            stoppingToken.Register(() =>
            {
                _logger.LogInformation($"Job is stopping.");
            });

            Task.Factory.StartNew(() => ConsumerAsync(10, stoppingToken), stoppingToken);

            _logger.LogInformation($"Job has stopped.");
            return Task.CompletedTask;
        }
    }
}
