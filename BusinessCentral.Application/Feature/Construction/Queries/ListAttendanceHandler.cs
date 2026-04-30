using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.Feature.Construction.Query;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public class ListAttendanceHandler : IRequestHandler<ListAttendanceQuery, Result<List<AttendanceDto>>>
    {
        private readonly IAttendanceRepository _repo;
        public ListAttendanceHandler(IAttendanceRepository repo) => _repo = repo;

        public async Task<Result<List<AttendanceDto>>> Handle(ListAttendanceQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListAttendanceAsync(request.ProjectId, request.From, request.To);
            return Result<List<AttendanceDto>>.Success(list);
        }
    }
}