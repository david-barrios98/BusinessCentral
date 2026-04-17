using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IRedisService
    {
        Task RevokeTokenAsync(string jti, TimeSpan expiration);
        Task<bool> IsTokenRevokedAsync(string jti);
    }
}
