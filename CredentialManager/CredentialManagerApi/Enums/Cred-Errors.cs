using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSCredentialManager.CredentialManagerApi.Enums
{
    public enum CRED_ERRORS : uint
    {
        ERROR_SUCCESS = 0x0,
        ERROR_INVALID_PARAMETER = 0x80070057,
        ERROR_INVALID_FLAGS = 0x800703EC,
        ERROR_NOT_FOUND = 0x80070490,
        ERROR_NO_SUCH_LOGON_SESSION = 0x80070520,
        ERROR_BAD_USERNAME = 0x8007089A,
        ERROR_UNKNOWN_ERROR = 0x1234567A
    }
}
