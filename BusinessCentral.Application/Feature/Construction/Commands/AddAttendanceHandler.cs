using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.Feature.Construction.Command;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public class AddAttendanceHandler : IRequestHandler<AddAttendanceCommand, Result<bool>>
    {
        private readonly IAttendanceRepository _repo;
        public AddAttendanceHandler(IAttendanceRepository repo) => _repo = repo;

        public async Task<Result<bool>> Handle(AddAttendanceCommand request, CancellationToken cancellationToken)
        {
            await _repo.InsertAttendanceAsync(request.ProjectId, request.Payload.UserId, request.Payload.DateWorked, request.Payload.Hours);
            return Result<bool>.Success(true);
        }
    }
}