using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Infrastructure.Persistence.Configuration
{
    public class FailedLoginOptions
    {
        public int MaxFailedAttempts { get; set; }
        public int LockoutDurationMinutes { get; set; }
        public string CacheKeyPrefix { get; set; }
    }
}
