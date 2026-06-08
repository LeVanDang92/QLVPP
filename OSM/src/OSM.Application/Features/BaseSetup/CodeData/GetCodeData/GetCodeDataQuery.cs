using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.BaseSetup.CodeData.GetCodeData
{
    public sealed record GetCodeDataQuery(string tableCode) : IQuery<List<CodeDataResponse>>;
}
