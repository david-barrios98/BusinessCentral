using MediatR;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Construction.Query
{
    public record ListApuItemsQuery(int ProjectId) : IRequest<Result<List<ApuItemDto>>>;
}