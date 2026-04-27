using BusinessCentral.Application.DTOs.Farm;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Farm.ProcessSteps;

public sealed record ListProcessStepsQuery(int CompanyId, long HarvestLotId) : IRequest<Result<List<CoffeeProcessStepDTO>>>;

