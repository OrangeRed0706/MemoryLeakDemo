using Library.Tracing.Enums;

namespace Library.Tracing.Options
{
    public sealed class TraceOption
    {
        /// <summary>
        /// Nullable
        /// </summary>
        public string ServiceNamePrefix { get; set; }

        /// <summary>
        /// Nullable
        /// </summary>
        public string ServiceNameSuffix { get; set; }

        /// <summary>
        /// Nullable, default Assembly Name.
        /// </summary>
        public string CustomServiceName { get; set; }


        /// <summary>
        /// Default true (use UdpReporter)
        /// </summary>
        public bool IsUseReporter { get; set; } = true;

        /// <summary>
        /// Jaeger Agent host, default localhost.
        /// </summary>
        public string AgentHost { get; set; } = "localhost";

        /// <summary>
        /// Jaeger Agent Udp port, default 6831.
        /// </summary>
        public int AgentPort { get; set; } = 6831;

        /// <summary>
        /// Jaeger Agent max size of packets, default 0 (when 0, Jaeger.Thrift.Senders.Internal.ThriftUdpClientTransport will set to 65000).
        /// </summary>
        public int SenderMaxPacketSize { get; set; } = 0;

        /// <summary>
        /// MaxQueueSize, default 100.
        /// </summary>
        public int MaxQueueSize { get; set; } = 100;

        /// <summary>
        /// FlushInterval(ms), default 1000(ms).
        /// </summary>
        public int FlushInterval { get; set; } = 1000;

        /// <summary>
        /// Defined sampling policy, default 0: None.
        /// </summary>
        public SamplingType SamplingType { get; set; }

        /// <summary>
        /// Send count per second, if 'SamplingType' has 'RateLimiting', default 10.
        /// </summary>
        public double RateLimiting { get; set; } = 10d;

        /// <summary>
        /// Randomly samples by percentage, if 'SamplingType' has 'Probabilistic', default 0.5.
        /// </summary>
        public double Probabilistic { get; set; } = 0.5d;
    }

}
