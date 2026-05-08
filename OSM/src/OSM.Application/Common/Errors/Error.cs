namespace OSM.Application.Common.Errors;

public sealed record ValidationError(string PropertyName, string ErrorMessage);

public sealed record Error(string Code,string Description,ErrorType Type,IReadOnlyList<ValidationError>? ValidationErrors = null)
{
    public static readonly Error None = new(
        string.Empty,
        string.Empty,
        ErrorType.None);

    public static Error Validation(IReadOnlyList<ValidationError> errors)
        => new(
            "Validation.Error",
            "One or more validation errors occurred.",
            ErrorType.Validation,
            errors);

    public static Error Unauthorized(string code, string description)
        => new(code, description, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string description)
        => new(code, description, ErrorType.Forbidden);

    public static Error NotFound(string code, string description)
        => new(code, description, ErrorType.NotFound);

    public static Error Conflict(string code, string description)
        => new(code, description, ErrorType.Conflict);

    public static Error Unexpected(string code, string description)
        => new(code, description, ErrorType.Unexpected);
}
