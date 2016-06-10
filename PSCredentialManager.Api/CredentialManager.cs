using PSCredentialManager.Common;
using PSCredentialManager.Common.Enum;
using PSCredentialManager.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PSCredentialManager.Api
{
    public class CredentialManager
    {
        public void WriteCred(NativeCredential credential)
        {
            // Write the info into the CredMan storage.
            bool written = Imports.CredWrite(ref credential, 0);
            int lastError = Marshal.GetLastWin32Error();
            if (!written)
            {
                string message = $"CredWrite failed with the error code {lastError}.";
                throw new Exception(message);
            }
        }

        public Credential ReadCred(string target, CredType type)
        {
            IntPtr nativeCredentialPointer;

            bool read = Imports.CredRead(target, type, 0, out nativeCredentialPointer);
            int lastError = Marshal.GetLastWin32Error();
            if (read)
            {
                using (CriticalCredentialHandle critCred = new CriticalCredentialHandle(nativeCredentialPointer))
                {
                    return critCred.GetCredential();
                }
            }
            else
            {
                string message;
                switch (lastError)
                {
                    case 1168:
                        message = $"Requested credential with target {target} was not found, error code {lastError}";
                        throw new CredentialNotFoundException(message);
                    default:
                        message = $"CredRead failed with the error code {lastError}.";
                        throw new Exception(message);
                }
                
            }
        }

        public void DeleteCred(string target, CredType type)
        {
            bool delete = Imports.CredDelete(target, type, 0);
            int lastError = Marshal.GetLastWin32Error();

            if (!delete)
            {
                string message = $"DeleteCred failed with the error code {lastError}.";
                throw new Exception(message);
            }
        }

        public IEnumerable<Credential> ReadCred()
        {
            int count;
            int flags;

            if (6 <= Environment.OSVersion.Version.Major)
            {
                flags = 0x1;
            }
            else
            {
                string message = "Retrieving all credentials is only possible on Windows version Vista or later.";
                throw new Exception(message);
            }

            IntPtr pCredentials;
            bool read = Imports.CredEnumerate(null, flags, out count, out pCredentials);
            int lastError = Marshal.GetLastWin32Error();

            if (read)
            {
                CriticalCredentialHandle credHandle = new CriticalCredentialHandle(pCredentials);
                return credHandle.GetCredentials(count);
            }
            else
            {
                string message = $"CredEnumerate failed with the error code {lastError}.";
                throw new Exception(message);
            }
        }

    }
}
