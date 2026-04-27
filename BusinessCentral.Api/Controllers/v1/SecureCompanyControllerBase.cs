using BusinessCentral.Api.Common;

namespace BusinessCentral.Api.Controllers.v1;

public abstract class SecureCompanyControllerBase : ApiControllerBase
{
    protected int CompanyId
    {
        get
        {
            var companyId = User.FindFirst("companyId")?.Value;
            return int.TryParse(companyId, out var id) ? id : 0;
        }
    }
}

