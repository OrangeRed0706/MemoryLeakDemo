using Library.Kafka;
using Library.Kafka.Common;
using Library.Kafka.Interfaces;
using Library.Tracing;
using MemoryLeakDemo.Contract;
using MemoryLeakDemo.Worker;
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

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
