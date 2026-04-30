using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Application.Constants
{
    public enum AccessResult
    {
        Success,            // Todo ok
        CompanyDisabled,    // La empresa está en is_active = false
        SubscriptionExpired, // Fecha de vencimiento pasada (402)
        ModuleNotIncluded,   // El plan no tiene ese módulo (403)
        Forbidden            // RBAC: usuario sin permiso (403)
    }
}
