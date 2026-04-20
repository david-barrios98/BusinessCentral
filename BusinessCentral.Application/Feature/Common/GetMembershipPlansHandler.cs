using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Common
{
    public class GetMembershipPlansHandler : IRequestHandler<GetMembershipPlansQuery, Result<List<MembershipPlanResponse>>>
    {
        private readonly ICommonRepository _repository;
        public GetMembershipPlansHandler(ICommonRepository repository) => _repository = repository;

        public async Task<Result<List<MembershipPlanResponse>>> Handle(GetMembershipPlansQuery request, CancellationToken cancellationToken)
        {
            var data = await _repository.ListMembershipPlansAsync();
            return Result<List<MembershipPlanResponse>>.Success(data);
        }
    }
}
