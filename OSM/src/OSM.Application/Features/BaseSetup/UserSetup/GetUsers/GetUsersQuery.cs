using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.BaseSetup.UserSetup.GetUsers
{
    public sealed record GetUsersQuery : IQuery<List<UserResponse>>;
}
