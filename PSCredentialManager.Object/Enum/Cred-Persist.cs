using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCredentialManager.Object.Enum
{
    public enum CRED_PERSIST : uint
    {
        SESSION = 1,
        LOCAL_MACHINE = 2,
        ENTERPRISE = 3
    }
}
