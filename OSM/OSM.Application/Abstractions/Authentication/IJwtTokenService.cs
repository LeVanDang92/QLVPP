namespace OSM.Application.Abstractions.Authentication
{
    public interface IJwtTokenService
    {
        string CreateAccessToken(string userId, string userName, IEnumerable<string> roles, IEnumerable<string> permissions);
        string CreateRefreshToken();
    }

}
