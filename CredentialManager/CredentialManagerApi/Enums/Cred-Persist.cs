using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSCredentialManager.CredentialManagerApi.Enums
{
    public enum CRED_PERSIST : uint
    {
        SESSION = 1,
        LOCAL_MACHINE = 2,
        ENTERPRISE = 3
    }
}
