using MediatR;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Common.Results;
using Microsoft.AspNetCore.Http;

public class CreateWorkLogHandler : IRequestHandler<CreateWorkLogNotesCommand, Result<WorkLogCreateResultDto>>
{
    private readonly IWorkLogRepository _repo;
    private readonly IFileStorageService _storage;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateWorkLogHandler(IWorkLogRepository repo, IFileStorageService storage, IHttpContextAccessor httpContextAccessor)
    {
        _repo = repo;
        _storage = storage;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<WorkLogCreateResultDto>> Handle(CreateWorkLogNotesCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
        int? userId = null;
        if (int.TryParse(userIdClaim, out var uid)) userId = uid;

        var workLogId = await _repo.InsertWorkLogAsync(request.ProjectId, userId, DateTime.UtcNow, request.Notes);

        if (request.Photos != null && request.Photos.Any())
        {
            foreach (var file in request.Photos)
            {
                using var stream = file.OpenReadStream();
                var path = await _storage.SaveFileAsync(stream, file.FileName, $"projects/{request.ProjectId}/worklogs");
                await _repo.InsertWorkLogPhotoAsync(workLogId!.Value, path, file.FileName);
            }
        }

        return Result<WorkLogCreateResultDto>.Success(new WorkLogCreateResultDto { WorkLogId = (int)workLogId });
    }
}