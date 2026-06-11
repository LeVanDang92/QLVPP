namespace OSM.Application.Features.BaseSetup.UserSetup
{
    public sealed record UpdateUserRequest(string UserName, string FullName, string Password, string Email, bool IsActive, string Department, string Role);
}
