using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace MemoryLeakDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnmanagedMemoryController : ControllerBase
    {
        private static List<IntPtr> _unmanagedLeakHolder = new();
        private const int BytesPerMegabyte = 1024 * 1024;

        [HttpPost("SimulateUnmanagedMemoryLeak")]
        public IActionResult SimulateUnmanagedMemoryLeak(int sizeInMb)
        {
            if (sizeInMb <= 0)
            {
                return BadRequest("Size must be greater than 0.");
            }

            int sizeInBytes = sizeInMb * BytesPerMegabyte;
            IntPtr pointer = Marshal.AllocHGlobal(sizeInBytes);
            _unmanagedLeakHolder.Add(pointer);

            return Ok($"Unmanaged memory leak of {sizeInMb}MB simulated.");
        }

        [HttpPost("CleanUnmanagedMemoryLeak")]
        public IActionResult CleanUnmanagedMemoryLeak()
        {
            foreach (var pointer in _unmanagedLeakHolder)
            {
                Marshal.FreeHGlobal(pointer);
            }
            _unmanagedLeakHolder = new List<IntPtr>();

            return Ok("Unmanaged memory leak cleaned.");
        }
    }
}
