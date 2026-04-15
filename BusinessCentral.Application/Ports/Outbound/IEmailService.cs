using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IEmailService
    {
        Task SendPasswordResetAsync(string toEmail, string subject, string htmlBody);
    }
}
