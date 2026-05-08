using ExcelDataReader;
using OSM.Application.Abstractions.Excel;
using System.Text;

namespace OSM.Infrastructure.Excel;

public sealed class ExcelDataReaderService : IExcelReaderService
{
    static ExcelDataReaderService()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public Task<ExcelReadResult> ReadAsync(
        Stream stream,
        string fileName,
        ExcelReadOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentNullException.ThrowIfNull(options);

        var extension = Path.GetExtension(fileName);

        if (!IsSupportedExtension(extension))
        {
            return Task.FromResult(new ExcelReadResult
            {
                Errors =
                [
                    new ExcelRowError(
                        0,
                        "File",
                        "Only .xlsx and .xls files are supported.")
                ]
            });
        }

        using var reader = ExcelReaderFactory.CreateReader(stream);

        var selectedSheetFound = false;

        do
        {
            cancellationToken.ThrowIfCancellationRequested();

            var currentSheetName = reader.Name;

            if (!ShouldReadSheet(currentSheetName, options.SheetName))
            {
                continue;
            }

            selectedSheetFound = true;

            return Task.FromResult(ReadCurrentSheet(
                reader,
                currentSheetName,
                options,
                cancellationToken));

        } while (reader.NextResult());

        return Task.FromResult(new ExcelReadResult
        {
            Errors =
            [
                new ExcelRowError(
                    0,
                    "Sheet",
                    string.IsNullOrWhiteSpace(options.SheetName)
                        ? "No worksheet found."
                        : $"Sheet '{options.SheetName}' was not found.")
            ]
        });
    }

    private static ExcelReadResult ReadCurrentSheet(
        IExcelDataReader reader,
        string sheetName,
        ExcelReadOptions options,
        CancellationToken cancellationToken)
    {
        var rows = new List<ExcelRow>();
        var errors = new List<ExcelRowError>();

        var headers = new List<ExcelHeader>();

        var currentRowNumber = 0;
        var dataRowsRead = 0;

        while (reader.Read())
        {
            cancellationToken.ThrowIfCancellationRequested();

            currentRowNumber++;

            if (currentRowNumber < options.HeaderRowNumber)
            {
                continue;
            }

            if (currentRowNumber == options.HeaderRowNumber)
            {
                headers = ReadHeaders(reader, options);

                if (headers.Count == 0)
                {
                    errors.Add(new ExcelRowError(
                        currentRowNumber,
                        "Header",
                        "Header row is empty."));

                    break;
                }

                ValidateRequiredColumns(headers, options, errors);

                continue;
            }

            if (currentRowNumber < options.StartDataRowNumber)
            {
                continue;
            }

            if (dataRowsRead >= options.MaxRows)
            {
                errors.Add(new ExcelRowError(
                    currentRowNumber,
                    "MaxRows",
                    $"Excel file exceeds max rows limit: {options.MaxRows}."));

                break;
            }

            if (options.StopWhenFirstColumnEmpty)
            {
                var firstColumnValue = GetCellStringValue(reader, 0, options);

                if (string.IsNullOrWhiteSpace(firstColumnValue))
                {
                    break;
                }
            }

            var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            foreach (var header in headers)
            {
                var value = GetCellStringValue(reader, header.ColumnIndex, options);
                values[header.Name] = string.IsNullOrWhiteSpace(value) ? null : value;
            }

            if (options.IgnoreEmptyRows && values.Values.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            rows.Add(new ExcelRow(currentRowNumber, values));
            dataRowsRead++;
        }

        return new ExcelReadResult
        {
            SheetName = sheetName,
            Rows = rows,
            Errors = errors
        };
    }

    private static List<ExcelHeader> ReadHeaders(
        IExcelDataReader reader,
        ExcelReadOptions options)
    {
        var headers = new List<ExcelHeader>();

        for (var columnIndex = 0; columnIndex < reader.FieldCount; columnIndex++)
        {
            var headerName = GetCellStringValue(reader, columnIndex, options);

            if (string.IsNullOrWhiteSpace(headerName))
            {
                continue;
            }

            if (headers.Any(x => string.Equals(x.Name, headerName, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            headers.Add(new ExcelHeader(
                columnIndex,
                headerName));
        }

        return headers;
    }

    private static void ValidateRequiredColumns(
        List<ExcelHeader> headers,
        ExcelReadOptions options,
        List<ExcelRowError> errors)
    {
        if (options.RequiredColumns is null || options.RequiredColumns.Length == 0)
        {
            return;
        }

        foreach (var requiredColumn in options.RequiredColumns)
        {
            var exists = headers.Any(x =>
                string.Equals(x.Name, requiredColumn, StringComparison.OrdinalIgnoreCase));

            if (!exists)
            {
                errors.Add(new ExcelRowError(
                    options.HeaderRowNumber,
                    requiredColumn,
                    $"Required column '{requiredColumn}' was not found."));
            }
        }
    }

    private static string? GetCellStringValue(
        IExcelDataReader reader,
        int columnIndex,
        ExcelReadOptions options)
    {
        if (columnIndex >= reader.FieldCount)
        {
            return null;
        }

        var value = reader.GetValue(columnIndex);

        if (value is null)
        {
            return null;
        }

        var text = value switch
        {
            DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            double number => number.ToString(System.Globalization.CultureInfo.InvariantCulture),
            float number => number.ToString(System.Globalization.CultureInfo.InvariantCulture),
            decimal number => number.ToString(System.Globalization.CultureInfo.InvariantCulture),
            bool boolean => boolean ? "true" : "false",
            _ => value.ToString()
        };

        return options.TrimValues
            ? text?.Trim()
            : text;
    }

    private static bool ShouldReadSheet(
        string currentSheetName,
        string? expectedSheetName)
    {
        return string.IsNullOrWhiteSpace(expectedSheetName)
            || string.Equals(currentSheetName, expectedSheetName, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSupportedExtension(string extension)
    {
        return string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".xls", StringComparison.OrdinalIgnoreCase);
    }

    private sealed record ExcelHeader(
        int ColumnIndex,
        string Name);
}