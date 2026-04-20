using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Common
{
    public record GetMembershipPlansQuery() : IRequest<Result<List<MembershipPlanResponse>>>;
    public record GetMembershipPlanByIdQuery(int Id) : IRequest<Result<MembershipPlanResponse>>;

}
