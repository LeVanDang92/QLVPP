using OSM.Application.Features.BaseSetup.UserSetup;

namespace OSM.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<UserResponse> UpdateUserAsync(UpdateUserRequest request);
        Task<bool> DeleteUserAsync(string userName);
    }
}
