using Microsoft.AspNetCore.Mvc;

namespace MemoryLeakDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemoryManagementController : ControllerBase
    {
        private static List<byte[]> _leakHolder = new();
        const int BytesPerMegabyte = 1024 * 1024;
        const int SmallObjectSize = 84999;

        [HttpPost("add-static-object-memory")]
        public IActionResult AddStaticObjectMemory(int sizeInMb)
        {
            if (sizeInMb == 0)
            {
                return BadRequest("Size cannot be 0.");
            }

            _leakHolder.Add(CreateByteArray(sizeInMb));

            return Ok($"Memory leak of {sizeInMb}MB simulated.");
        }

        [HttpPost("release-static-object-memory")]
        public IActionResult ReleaseStaticObjectMemory()
        {
            _leakHolder = new List<byte[]>();

            return Ok("Memory leak cleaned.");
        }

        [HttpPost("create-multiple-small-objects-in-soh")]
        public IActionResult CreateMultipleSmallObjectsInSOH(int totalSizeInMb)
        {
            var totalBytes = BytesPerMegabyte * totalSizeInMb;
            var rounds = totalBytes / SmallObjectSize;

            for (var round = 0; round < rounds; round++)
            {
                var smallObject = new byte[SmallObjectSize];
            }

            return Ok($"{rounds} small objects of {SmallObjectSize} bytes each created and stored in SOH.");
        }


        [HttpPost("call-gc")]
        public IActionResult CallGc()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            return Ok("Garbage collected.");
        }

        [HttpPost("create-large-object-in-loh")]
        public IActionResult CreateLargeObjectInLOH(int sizeInMb)
        {
            var largeObject = CreateByteArray(sizeInMb);

            return Ok($"Large object of {sizeInMb}MB created and stored in LOH.");
        }

        private byte[] CreateByteArray(int sizeInMb)
        {
            return new byte[BytesPerMegabyte * sizeInMb];
        }
    }
}
