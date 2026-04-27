using BusinessCentral.Api.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusinessCentral.Api.Controllers.v1.Hr;

public abstract class HrControllerBase : ApiControllerBase
{
    protected int CompanyId
    {
        get
        {
            var companyId = User.FindFirst("companyId")?.Value;
            if (int.TryParse(companyId, out var id))
                return id;
            return 0;
        }
    }
}

