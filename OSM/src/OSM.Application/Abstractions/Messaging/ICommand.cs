using MediatR;
using OSM.Application.Common;

namespace OSM.Application.Abstractions.Messaging
{
    public interface ICommand : IRequest<Result>;
    public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
}
