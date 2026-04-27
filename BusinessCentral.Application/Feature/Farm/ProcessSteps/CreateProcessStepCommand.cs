using BusinessCentral.Application.DTOs.Farm;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Farm.ProcessSteps;

public sealed record CreateProcessStepCommand(int CompanyId, CoffeeProcessStepDTO Step) : IRequest<Result<long>>;

