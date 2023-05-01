using Library.Kafka;
using Library.Kafka.Common;
using Library.Kafka.Interfaces;
using Library.Tracing;
using MemoryLeakDemo.Contract;
using MemoryLeakDemo.Worker;
using Prometheus;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CustomerService
builder.Services.AddHostedService<ConsumerWorker>();


builder.Services.AddSingleton<ICustomSerializer<GenerateMessage>, CustomSerializer<GenerateMessage>>();

//Library
builder.Services.AddTracing(builder.Configuration); 
builder.Services.AddKafkaProducerService(builder.Configuration);
builder.Services.AddKafkaConsumerService(builder.Configuration);


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Use the Prometheus
var counter = Metrics.CreateCounter("demo_counter", "Counts",
    new CounterConfiguration
    {
        LabelNames = new[] { "method", "endpoint" },
        SuppressInitialValue = true,
        ExemplarBehavior = ExemplarBehavior.NoExemplars()
    });

app.Use((context, next) =>
{
    var sw = Stopwatch.StartNew();
    try
    {
        return next();
    }
    finally
    {
        sw.Stop();
        counter.WithLabels(context.Request.Method, context.Request.Path).Inc(sw.ElapsedMilliseconds);
    }
});
app.UseMetricServer();
app.UseHttpMetrics();

app.Run();
