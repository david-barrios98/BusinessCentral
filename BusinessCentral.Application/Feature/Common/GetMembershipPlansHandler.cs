using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
