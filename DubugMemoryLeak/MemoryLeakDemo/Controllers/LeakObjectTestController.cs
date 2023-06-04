using MemoryLeakDemo.Model;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace MemoryLeakDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeakObjectTestController : ControllerBase
    {
        private static LeakyObject _leakyObject;

        public LeakObjectTestController()
        {

        }

        [HttpPost]
        [Route("create-leaky-object")]
        public IActionResult CreateLeakyObject([FromForm] int sizeInMb)
        {
            _leakyObject = new LeakyObject(sizeInMb);

            return Ok("Created a leaky object of " + sizeInMb + "MB.");
        }

        [HttpPost]
        [Route("clear-leaky-object")]
        public IActionResult ClearLeakyObject()
        {
            if (_leakyObject == null)
            {
                return Ok("No leaky object to clear.");
            }
            _leakyObject = null;

            return Ok("Cleared the reference to the leaky object.");
        }
    }
}
