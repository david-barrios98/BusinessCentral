public class WorkLogDto
{
    public int WorkLogId { get; set; }
    public int ProjectId { get; set; }
    public int? LoggedByUserId { get; set; }
    public DateTime LogDate { get; set; }
    public string? Notes { get; set; }
}

public class WorkLogCreateResultDto
{
    public int WorkLogId { get; set; }
}