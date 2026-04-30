using BusinessCentral.Application.Feature.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

public record CreateWorkLogCommand(int ProjectId, string Notes, List<IFormFile>? Photos) : IRequest<Result<WorkLogCreateResultDto>>;