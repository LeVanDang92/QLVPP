using Microsoft.AspNetCore.Mvc;
using OSM.Application.Common;

namespace OSM.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        protected IActionResult HandleResult(Result result) => result.IsSuccess ? Ok() : BadRequest(result.Error);

        protected IActionResult HandleResult<T>(Result<T> result) => result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
