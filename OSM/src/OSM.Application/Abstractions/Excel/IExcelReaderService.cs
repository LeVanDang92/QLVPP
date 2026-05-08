namespace OSM.Application.Abstractions.Excel;

public interface IExcelReaderService
{
    Task<ExcelReadResult> ReadAsync(
        Stream stream,
        string fileName,
        ExcelReadOptions options,
        CancellationToken cancellationToken = default);
}
