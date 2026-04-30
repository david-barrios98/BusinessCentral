using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Feature.Construction.Query
{
    public record ListAttendanceQuery(int ProjectId, DateTime? From, DateTime? To) : IRequest<Result<List<AttendanceDto>>>;
}