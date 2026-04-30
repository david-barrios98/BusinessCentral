using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

public record CreateWorkLogNotesCommand(int ProjectId, string Notes, List<IFormFile>? Photos) : IRequest<Result<WorkLogCreateResultDto>>;

public record CreateWorkLogCommand(int CompanyId, WorkLogDTO dto) : IRequest<Result<WorkLogCreateResultDto>>;