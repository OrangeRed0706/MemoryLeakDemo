using Library.Tracing.Interface;
using OpenTracing;
using OpenTracing.Propagation;

namespace Library.Tracing.Services
{
    internal sealed class TraceManager : ITraceManager
    {
        private readonly ITracer _tracer;

        public TraceManager(ITracer tracer)
        {
            _tracer = tracer;
        }

        ISpanContext ITraceManager.ExtractHeader(IDictionary<string, string> header)
        {
            return _tracer.Extract(BuiltinFormats.HttpHeaders, new TextMapExtractAdapter(header));
        }

        ISpanContext ITraceManager.InjectHeader(ISpanContext span, IDictionary<string, string> inject)
        {
            _tracer.Inject(span, BuiltinFormats.HttpHeaders, new TextMapInjectAdapter(inject));
            return span;
        }

        IScope ITraceManager.StartScopeTrace(string operationName, Action<ISpanBuilder> builderAction)
        {
            var builder = _tracer.BuildSpan(operationName);
            builderAction?.Invoke(builder);
            return builder.StartActive(true);
        }

        ISpanBuilder ITraceManager.CreateSpanBuilder(string operationName, Action<ISpanBuilder> builderAction)
        {
            return _tracer.BuildSpan(operationName);
        }

        ISpan ITraceManager.GetCurrentSpan()
        {
            return _tracer.ActiveSpan;
        }

        string ITraceManager.GetCurrentSpanId()
        {
            return _tracer.ActiveSpan?.Context?.SpanId;
        }

        string ITraceManager.GetCurrentTraceId()
        {
            return _tracer.ActiveSpan?.Context?.TraceId;
        }
    }

}
