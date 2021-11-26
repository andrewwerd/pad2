using Common;
using Microsoft.AspNetCore.Mvc;
using SyncNode.Services;

namespace SyncNode.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly SyncWorkJobServices _service;

        public SyncController(SyncWorkJobServices service) => _service = service;

        [HttpPost]
        public IActionResult Sync(SyncEntity entity)
        {
            _service.AddItem(entity);

            return Ok();
        }
    }
}