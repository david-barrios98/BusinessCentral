using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IHashPasswordService
    {
        bool Verify(string password, string hash);

        string Hash(string password);
    }
}
