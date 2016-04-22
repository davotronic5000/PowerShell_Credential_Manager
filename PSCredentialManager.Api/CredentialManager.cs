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
        public void WriteCred(NativeCredential Credential)
        {
            // Write the info into the CredMan storage.
            bool written = Imports.CredWrite(ref Credential, 0);
            int LastError = Marshal.GetLastWin32Error();
            if (!written)
            {
                string message = string.Format("CredWrite failed with the error code {0}.", LastError);
                throw new Exception(message);
            }
        }

        public Credential ReadCred(string target, Cred_Type type)
        {
            IntPtr nativeCredentialPointer;

            bool Read = Imports.CredRead(target, type, 0, out nativeCredentialPointer);
            int LastError = Marshal.GetLastWin32Error();
            if (Read)
            {
                using (CriticalCredentialHandle critCred = new CriticalCredentialHandle(nativeCredentialPointer))
                {
                    return critCred.GetCredential();
                }
            }
            else
            {
                string message;
                switch (LastError)
                {
                    case 1168:
                        message = string.Format("Requested credential with target {0} was not found, error code {1}", target, LastError);
                        throw new CredentialNotFoundException(message);
                    default:
                        message = string.Format("CredRead failed with the error code {0}.", LastError);
                        throw new Exception(message);
                }
                
            }
        }

        public void DeleteCred(string target, Cred_Type type)
        {
            bool Delete = Imports.CredDelete(target, type, 0);
            int LastError = Marshal.GetLastWin32Error();

            if (!Delete)
            {
                string message = string.Format("DeleteCred failed with the error code {0}.", LastError);
                throw new Exception(message);
            }
        }

        public IEnumerable<Credential> ReadCred()
        {
            int count = 0;
            int flags = 0x0;

            if (6 <= Environment.OSVersion.Version.Major)
            {
                flags = 0x1;
            }
            else
            {
                string message = "Retrieving all credentials is only possible on Windows version Vista or later.";
                throw new Exception(message);
            }

            IntPtr pCredentials = IntPtr.Zero;
            bool Read = Imports.CredEnumerate(null, flags, out count, out pCredentials);
            int LastError = Marshal.GetLastWin32Error();

            if (Read)
            {
                CriticalCredentialHandle CredHandle = new CriticalCredentialHandle(pCredentials);
                return CredHandle.GetCredentials(count);
            }
            else
            {
                string message = string.Format("CredEnumerate failed with the error code {0}.", LastError);
                throw new Exception(message);
            }
        }

    }
}
