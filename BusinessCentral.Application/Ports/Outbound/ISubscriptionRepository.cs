using BusinessCentral.Application.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface ISubscriptionRepository
    {
        Task<AccessResult> CheckAccessAsync(int companyId, string? moduleName);
    }
}
