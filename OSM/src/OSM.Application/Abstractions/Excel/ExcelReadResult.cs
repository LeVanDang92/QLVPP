namespace OSM.Application.Abstractions.Excel;

public sealed class ExcelReadResult
{
    public string? SheetName { get; init; }

    public List<ExcelRow> Rows { get; init; } = [];

    public List<ExcelRowError> Errors { get; init; } = [];

    public bool IsSuccess => Errors.Count == 0;
}
