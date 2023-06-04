using Library.Kafka.Interfaces;
using Library.Kafka.Options;
using Library.Kafka.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Library.Kafka
{
    public static class ServiceProvider
    {
        public static IServiceCollection AddKafkaConsumerService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddKafkaConsumerConfig(configuration);
            services.TryAddTransient<IConsumerService, ConsumerService>();
            return services;
        }

        public static IServiceCollection AddKafkaProducerService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddKafkaProducerConfig(configuration);
            services.TryAddSingleton<IProducerService, ProducerService>();
            return services;
        }

        private static void AddKafkaProducerConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ProducerOptions>(configuration.GetSection("Kafka:Producers"));
        }

        private static void AddKafkaConsumerConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConsumerOptions>(configuration.GetSection("Kafka:Consumers"));
        }
    }
}
