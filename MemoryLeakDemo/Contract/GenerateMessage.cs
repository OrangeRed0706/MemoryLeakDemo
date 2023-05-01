namespace MemoryLeakDemo.Contract
{
    public class GenerateMessage : RetriableTraceMessage
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
