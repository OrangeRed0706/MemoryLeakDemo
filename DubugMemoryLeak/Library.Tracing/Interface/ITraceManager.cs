using OpenTracing;

namespace Library.Tracing.Interface
{
    public interface ITraceManager
    {
        IScope StartScopeTrace(string operationName, Action<ISpanBuilder> builderAction = null);

        ISpanContext ExtractHeader(IDictionary<string, string> header);

        ISpanContext InjectHeader(ISpanContext span, IDictionary<string, string> inject);

        ISpanBuilder CreateSpanBuilder(string operationName, Action<ISpanBuilder> builderAction = null);

        ISpan GetCurrentSpan();

        string GetCurrentSpanId();

        string GetCurrentTraceId();
    }
}
