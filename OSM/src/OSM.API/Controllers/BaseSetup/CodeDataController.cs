using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSM.Application.Abstractions.Caching;
using OSM.Application.Features.BaseSetup.CodeData;
using OSM.Application.Features.BaseSetup.CodeData.GetCodeData;

namespace OSM.API.Controllers.BaseSetup
{
    [Authorize]
    public class CodeDataController(ISender sender, ICacheService cacheService) : ApiController
    {
        [HttpGet("{tableCode}")]
        public async Task<IActionResult> CodeData(string tableCode, CancellationToken cancellationToken)
        {
            var cachedData = await cacheService.GetAsync<List<CodeDataResponse>>("cache_" + tableCode, cancellationToken);

            if (cachedData != null)
            {
                return Ok(cachedData);
            }

            var result = await sender.Send(new GetCodeDataQuery(tableCode), cancellationToken);

            await cacheService.SetAsync("cache_" + tableCode, result.Value, TimeSpan.FromHours(1), cancellationToken);

            return HandleResult(result);
        }
    }
}
