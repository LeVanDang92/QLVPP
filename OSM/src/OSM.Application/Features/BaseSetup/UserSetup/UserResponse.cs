namespace OSM.Application.Features.BaseSetup.UserSetup
{
    public sealed record UserResponse(string UserId,string FullName, string UserName, string Email,string PasswordShow, string Role, string Department, bool IsActive, DateTimeOffset CreatedAt, string CreatedBy, DateTimeOffset? ModifiedAt, string? ModifiedBy);
}
