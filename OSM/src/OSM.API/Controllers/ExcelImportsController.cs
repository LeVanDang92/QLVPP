using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSM.Application.Abstractions.Excel;

namespace OSM.API.Controllers;

[ApiVersion("1.0")]
[Authorize]
public sealed class ExcelImportsController : ApiController
{
    private readonly IExcelReaderService _excelReaderService;

    public ExcelImportsController(IExcelReaderService excelReaderService)
    {
        _excelReaderService = excelReaderService;
    }

    [HttpPost("preview")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Preview(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("Excel file is required.");
        }

        var extension = Path.GetExtension(file.FileName);

        if (!string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(extension, ".xls", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only .xlsx and .xls files are supported.");
        }

        await using var stream = file.OpenReadStream();

        var result = await _excelReaderService.ReadAsync(
            stream,
            file.FileName,
            new ExcelReadOptions
            {
                HeaderRowNumber = 1,
                StartDataRowNumber = 2,
                MaxRows = 5_000,
                StopWhenFirstColumnEmpty = true,
                RequiredColumns =
                [
                    "Code",
                    "Name",
                    "StockQuantity"
                ]
            },
            cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                result.SheetName,
                result.Errors
            });
        }

        return Ok(new
        {
            result.SheetName,
            TotalRows = result.Rows.Count,
            Rows = result.Rows.Take(20)
        });
    }
}