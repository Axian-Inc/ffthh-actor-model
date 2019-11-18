using Axian.ActorModel.Website.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Axian.ActorModel.Website.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SysController : ControllerBase
    {
        private readonly AxSystem _ax;
        public SysController(AxSystem ax)
        {
            _ax = ax;
        }

        public IActionResult Get()
        {
            var model = new
            {
                _ax.Sys.Name,
                StartTime = _ax.Sys.StartTime.FromUnixTimeMilliseconds(),
                _ax.Sys.Uptime
            };

            return Ok(model);
        }
    }
}
