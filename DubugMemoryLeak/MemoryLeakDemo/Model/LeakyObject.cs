namespace MemoryLeakDemo.Model
{
    public class LeakyObject
    {
        private static List<byte[]> _buffer = new();

        public LeakyObject(int sizeInMb)
        {
            _buffer.Add(new byte[1024 * 1024 * sizeInMb]);
        }

        ~LeakyObject()
        {
            _buffer = new();
            Console.WriteLine("LeakyObject is being cleaned up by the GC.");
            GC.Collect();
        }
    }
}
