using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Common;

public sealed class GetMembershipPlanModulesHandler
    : IRequestHandler<GetMembershipPlanModulesQuery, Result<List<MembershipPlanModuleResponse>>>
{
    private readonly ICommonRepository _repository;

    public GetMembershipPlanModulesHandler(ICommonRepository repository) => _repository = repository;

    public async Task<Result<List<MembershipPlanModuleResponse>>> Handle(
        GetMembershipPlanModulesQuery request,
        CancellationToken cancellationToken)
    {
        var list = await _repository.ListPlanModulesAsync(request.MembershipPlanId);
        return Result<List<MembershipPlanModuleResponse>>.Success(list);
    }
}
