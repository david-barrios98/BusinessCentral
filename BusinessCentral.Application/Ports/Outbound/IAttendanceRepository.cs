using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IAttendanceRepository
    {
        Task InsertAttendanceAsync(int projectId, int userId, DateTime dateWorked, decimal hours);
        Task<List<AttendanceDto>> ListAttendanceAsync(int projectId, DateTime? from = null, DateTime? to = null);
    }
}