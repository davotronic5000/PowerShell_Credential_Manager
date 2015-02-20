using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSCredentialManager.CredentialManagerApi.Enums
{
    public enum CRED_FLAGS : uint
    {
        NONE = 0x0,
        PROMPT_NOW = 0x2,
        USERNAME_TARGET = 0x4
    }
}
