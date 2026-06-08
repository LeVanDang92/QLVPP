using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSM.Application.Features.BaseSetup.CodeData.GetCodeData;

namespace OSM.API.Controllers.BaseSetup
{
    [Authorize]
    public class CodeDataController(ISender sender) : ApiController
    {
        [HttpGet("{tableCode}")]
        public async Task<IActionResult> CodeData(string tableCode, CancellationToken cancellationToken)
        {
           var result = await sender.Send(new GetCodeDataQuery(tableCode), cancellationToken);
            return HandleResult(result);
        }
    }
}
