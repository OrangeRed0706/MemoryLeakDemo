using Jaeger.Thrift;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.IO;

namespace MemoryLeakDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class SystemInfoController : ControllerBase
{
    [HttpGet]
    [Route("memory")]
    public IActionResult GetMemoryUsage()
    {
        var process = System.Diagnostics.Process.GetCurrentProcess();
        var memoryInBytes = process.WorkingSet64;
        var memoryInMb = memoryInBytes / 1024 / 1024;

        long totalMemoryInBytes;
        using (var reader = new StreamReader("/sys/fs/cgroup/memory/memory.limit_in_bytes"))
        {
            totalMemoryInBytes = long.Parse(reader.ReadToEnd().Trim());
        }

        var totalMemoryInMb = totalMemoryInBytes / 1024 / 1024;

        return Ok(new
        {
            TotalMemoryInMB = totalMemoryInMb,
            MemoryUsageInMB = memoryInMb,
            MemoryUsagePercentage = Math.Round(((double)memoryInMb / totalMemoryInMb) * 100, 2)
        });
    }

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


    [HttpPost]
    [Route("gc-collect")]
    public IActionResult TriggerGC()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();

        return Ok("GC triggered.");
    }
}