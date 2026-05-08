namespace OSM.Application.Abstractions.Excel;

public sealed class ExcelRow
{
    public ExcelRow(
        int rowNumber,
        Dictionary<string, string?> values)
    {
        RowNumber = rowNumber;
        Values = values;
    }

    public int RowNumber { get; }

    public Dictionary<string, string?> Values { get; }

    public string? GetValue(string columnName)
    {
        return Values.TryGetValue(columnName, out var value)
            ? value
            : null;
    }
}