namespace OSM.Application.Abstractions.Excel;

public sealed class ExcelReadOptions
{
    public string? SheetName { get; init; }

    public int HeaderRowNumber { get; init; } = 1;

    public int StartDataRowNumber { get; init; } = 2;

    public bool StopWhenFirstColumnEmpty { get; init; } = true;

    public int MaxRows { get; init; } = 10_000;

    public bool TrimValues { get; init; } = true;

    public bool IgnoreEmptyRows { get; init; } = true;

    public string[]? RequiredColumns { get; init; }
}
