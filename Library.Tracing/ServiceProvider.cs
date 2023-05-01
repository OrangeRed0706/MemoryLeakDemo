using System.Reflection;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using Library.Tracing.Enums;
using Library.Tracing.Interface;
using Library.Tracing.Options;
using Library.Tracing.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;

namespace Library.Tracing
{
    public static class ServiceProvider
    {
        public static IServiceCollection AddTracing(this IServiceCollection services, IConfiguration configuration, string section = "Trace", Action<IOpenTracingBuilder> builderAction = default, bool ignoreHealthCheck = true, string healthCheckPrefix = "/health-check")
        {
            var option = configuration.GetSection(section).Get<TraceOption>();

            // Use "OpenTracing.Contrib.NetCore" to automatically generate spans for ASP.NET Core, Entity Framework Core, ...
            // See https://github.com/opentracing-contrib/csharp-netcore for details.
            services.AddOpenTracing(builder =>
            {
                builderAction?.Invoke(builder);

                if (ignoreHealthCheck)
                {
                    builder = builder.IgnoreHealthCheck(healthCheckPrefix);
                }
            });

            // Adds the Jaeger Tracer.
            services.AddSingleton((Func<IServiceProvider, ITracer>)(serviceProvider =>
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var appName = string.IsNullOrEmpty(option.CustomServiceName) ? Assembly.GetEntryAssembly().GetName().Name : option.CustomServiceName;
                var serviceName = $"{option.ServiceNamePrefix}{appName}{option.ServiceNameSuffix}";
                var reporter = GetReporter(option, loggerFactory);
                var sampler = GetSampler(option);
                var tracer = new Tracer.Builder(serviceName)
                        .WithLoggerFactory(loggerFactory)
                        .WithReporter(reporter)
                        .WithSampler(sampler)
                        .WithTraceId128Bit()
                        .Build();

                GlobalTracer.Register(tracer);

                return tracer;
            }));

            services.AddSingleton<ITraceManager, TraceManager>();

            return services;
        }

        private static IReporter GetReporter(TraceOption option, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("Higgs.Kernel.Monitoring.Tracing");

            if (!option.IsUseReporter)
            {
                logger.LogInformation($"use {nameof(NoopReporter)} for tracing.");
                return new NoopReporter();
            }

            IReporter reporter;
            try
            {
                reporter = new RemoteReporter.Builder()
                    .WithSender(new UdpSender(option.AgentHost, option.AgentPort, option.SenderMaxPacketSize))
                    .WithMaxQueueSize(option.MaxQueueSize)
                    .WithFlushInterval(TimeSpan.FromMilliseconds(option.FlushInterval))
                    .Build();

                logger.LogInformation($"use {nameof(RemoteReporter)}-{nameof(UdpSender)} for tracing.");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"cannot get {nameof(RemoteReporter)}, use {nameof(NoopReporter)} for tracing.");
                reporter = new NoopReporter();
            }

            return reporter;
        }

        public static IOpenTracingBuilder IgnoreHealthCheck(this IOpenTracingBuilder builder, string healthCheckPrefix = "/health-check")
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureAspNetCore(option => option.Hosting.IgnorePatterns.Add(request => request.Request.Path.Value?.StartsWith(healthCheckPrefix) == true));

            return builder;
        }

        private static ISampler GetSampler(TraceOption option)
        {
            switch (option.SamplingType)
            {
                case SamplingType.RateLimiting:
                    return new RateLimitingSampler(option.RateLimiting);
                case SamplingType.Probabilistic:
                    return new ProbabilisticSampler(option.Probabilistic);
                case SamplingType.Combine:
                    return new GuaranteedThroughputSampler(option.Probabilistic, option.RateLimiting);
                case SamplingType.None:
                default:
                    return new ConstSampler(true);
            }
        }
    }
}
