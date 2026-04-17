
namespace BusinessCentral.Infrastructure.Models
{
    public class FailedLoginOptions
    {
        public int MaxFailedAttempts { get; set; }
        public int LockoutDurationMinutes { get; set; }
        public string CacheKeyPrefix { get; set; }
    }
}
