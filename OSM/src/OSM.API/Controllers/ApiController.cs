using Microsoft.AspNetCore.Mvc;

namespace OSM.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public abstract class ApiController : BaseController
    {
        
    }
}
