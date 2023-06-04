using Microsoft.AspNetCore.Mvc;

namespace MemoryLeakDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class SystemInfoController : ControllerBase
{
    [HttpGet]
    [Route("get-gc-info")]
    public IActionResult GetGCInfo()
    {
        var gen0CollectionCount = GC.CollectionCount(0);
        var gen1CollectionCount = GC.CollectionCount(1);
        var gen2CollectionCount = GC.CollectionCount(2);
        var totalMemoryInMB = GC.GetTotalMemory(false) / 1024 / 1024;
        var totalAllocatedBytesInMB = GC.GetTotalAllocatedBytes(false) / 1024 / 1024;

        var allocatedBytesForCurrentThreadInMB = GC.GetAllocatedBytesForCurrentThread() / 1024 / 1024;

        return Ok(new
        {
            Gen0CollectionCount = gen0CollectionCount,
            Gen1CollectionCount = gen1CollectionCount,
            Gen2CollectionCount = gen2CollectionCount,
            TotalMemoryInMB = totalMemoryInMB,
            TotalAllocatedBytesInMB = totalAllocatedBytesInMB,
            AllocatedBytesForCurrentThreadInMB = allocatedBytesForCurrentThreadInMB
        });
    }
}