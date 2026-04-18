using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Application.Feature.Common
{
    public class GetMembershipPlanByIdHandler : IRequestHandler<GetMembershipPlanByIdQuery, Result<MembershipPlanResponse>>
    {
        private readonly ICommonRepository _repository;
        public GetMembershipPlanByIdHandler(ICommonRepository repository) => _repository = repository;

        public async Task<Result<MembershipPlanResponse>> Handle(GetMembershipPlanByIdQuery request, CancellationToken cancellationToken)
        {
            var plan = await _repository.GetMembershipPlanByIdAsync(request.Id);

            if (plan == null)
                return Result<MembershipPlanResponse>.Failure("Plan de membresía no encontrado.", "PLAN_NOT_FOUND", "NotFound");

            return Result<MembershipPlanResponse>.Success(plan);
        }
    }

    public class GetMembershipPlanByIdValidator : AbstractValidator<GetMembershipPlanByIdQuery>
    {
        public GetMembershipPlanByIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty().GreaterThan(0).WithMessage("Debe proporcionar un ID de plan válido.");
        }
    }
}
