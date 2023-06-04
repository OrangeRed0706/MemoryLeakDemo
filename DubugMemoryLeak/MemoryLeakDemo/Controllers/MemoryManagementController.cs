using Microsoft.AspNetCore.Mvc;
using System.Runtime;

namespace MemoryLeakDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemoryManagementController : ControllerBase
    {
        private static List<byte[]> _leakHolder = new();
        const int BytesPerMegabyte = 1024 * 1024;
        const int SmallObjectSize = 84000;


        /// <summary>
        /// Creates a byte array of the specified size in MB.
        /// </summary>
        /// <param name="sizeInMb"></param>
        /// <returns></returns>
        [HttpPost("SimulateMemoryLeak")]
        public IActionResult SimulateMemoryLeak(int sizeInMb)
        {
            if (sizeInMb == 0)
            {
                return BadRequest("Size cannot be 0.");
            }

            _leakHolder.Add(CreateByteArray(sizeInMb));

            return Ok($"Memory leak of {sizeInMb}MB simulated.");
        }

        /// <summary>
        /// 建立多個小物件
        /// </summary>
        /// <param name="totalSizeInMb"></param>
        /// <returns></returns>
        [HttpPost("CreateMultipleSmallObjectsInSOH")]
        public IActionResult CreateMultipleSmallObjectsInSOH(int totalSizeInMb)
        {
            var totalBytes = BytesPerMegabyte * totalSizeInMb;
            var rounds = totalBytes / SmallObjectSize;

            for (var round = 0; round < rounds; round++)
            {
                var smallObject = new byte[SmallObjectSize];
                _leakHolder.Add(smallObject);
            }

            return Ok($"{rounds} small objects of {SmallObjectSize} bytes each created and stored in SOH.");
        }

        [HttpPost("CleanMemoryLeak")]
        public IActionResult CleanMemoryLeak()
        {
            _leakHolder = new List<byte[]>();

            return Ok("Memory leak cleaned.");
        }

        [HttpPost("CollectGarbage")]
        public IActionResult CollectGarbage()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            return Ok("Garbage collected.");
        }

        [HttpPost("CreateLargeObjectInLOH")]
        public IActionResult CreateLargeObjectInLOH(int sizeInMb)
        {
            var largeObject = new byte[BytesPerMegabyte * sizeInMb];

            return Ok($"Large object of {sizeInMb}MB created and stored in LOH.");
        }

        [HttpPost("TestLOHReserveMemory")]
        public async Task<IActionResult> TestLOHReserveMemory(int totalSizeInMb)
        {
            Func<int, byte[]> createBigArray = (mb) => new byte[mb * BytesPerMegabyte];
            var bytesList = new List<byte[]>();
            await Task.Delay(2000);
            var i = 1;
            try
            {
                while (true)
                {
                    bytesList.Add(createBigArray(totalSizeInMb));
                    i++;
                }
            }
            catch (Exception ex)
            {
                DumpMemSize($"{i}*{totalSizeInMb}MB");
                Console.WriteLine("ERROR-" + ex.Message);
            }

            GCSettings.LatencyMode = GCLatencyMode.Batch;
            GC.Collect();
            return Ok();
        }

        [HttpPost("TestLOHFragmentation")]
        public async Task<IActionResult> TestLOHFragmentation()
        {
            var dataObjects = new List<object>();
            var fragObjects = new List<byte[]>();
            Func<int, byte[]> createBigArray = (mb) => new byte[mb * BytesPerMegabyte];
            for (var i = 0; i < 13; i++)
            {
                dataObjects.Add(createBigArray(80));
                fragObjects.Add(createBigArray(16));
                DumpMemSize($"Objects added - {i} ");
            }
            dataObjects.Clear();
            DumpMemSize($"dataObjects cleared");
            GC.Collect();
            DumpMemSize($"GC.Collect()");
            dataObjects.Add(createBigArray(300));
            return Ok();
        }

        [HttpPost("TestLOHCompactionFragmentation")]
        public IActionResult TestLOHCompactionFragmentation()
        {
            var dataObjects = new List<object>();
            var fragObjects = new List<byte[]>();
            Func<int, byte[]> createBigArray = (mb) => new byte[mb * BytesPerMegabyte];
            for (var i = 0; i < 13; i++)
            {
                dataObjects.Add(createBigArray(80));
                fragObjects.Add(createBigArray(16));
                DumpMemSize($"Objects added - {i} ");
            }
            dataObjects.Clear();
            DumpMemSize($"dataObjects cleared");
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
            DumpMemSize($"GC.Collect()");
            dataObjects.Add(createBigArray(300));
            return Ok();
        }

        private void DumpMemSize(string msg)
        {
            var memSz = GC.GetTotalMemory(false) / 1024 / 1024;
            Console.WriteLine($"Managed Heap = {memSz}MB {msg}");
        }

        private byte[] CreateByteArray(int sizeInMb)
        {
            return new byte[BytesPerMegabyte * sizeInMb];
        }
    }
}
