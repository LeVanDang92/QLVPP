namespace OSM.Application.Abstractions.Excel;

public sealed record ExcelRowError(
    int RowNumber,
    string ColumnName,
    string Message);