using MediatR;
using OSM.Application.Common;

namespace OSM.Application.Abstractions.Messaging
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
}
