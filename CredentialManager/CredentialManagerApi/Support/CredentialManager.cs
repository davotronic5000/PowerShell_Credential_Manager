using PSCredentialManager.CredentialManagerApi.Enums;
using PSCredentialManager.CredentialManagerApi.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PSCredentialManager.CredentialManagerApi.Support
{
    class CredentialManager
    {
        public int WriteCred(NativeCredential Credential)
        {
            // Write the info into the CredMan storage.
            bool written = Imports.CredWrite(ref Credential, 0);
            int LastError = Marshal.GetLastWin32Error();
            if (written)
            {
                return 0;
            }
            else
            {
                string message = string.Format("CredWrite failed with the error code {0}.", LastError);
                throw new Exception(message);
            }
        }

        public Credential ReadCred(string target, CRED_TYPE type)
        {
            IntPtr NativeCredentialPointer;
            Credential credential = new Credential();

            bool Read = Imports.CredRead(target, type, 0, out NativeCredentialPointer);
            int LastError = Marshal.GetLastWin32Error();
            if (Read)
            {
                using (CriticalCredentialHandle critCred = new CriticalCredentialHandle(NativeCredentialPointer))
                {
                    credential = critCred.GetCredential();
                }
                return credential;
            }
            else
            {
                string message = string.Format("CredRead failed with the error code {0}.", LastError);
                throw new Exception(message);
            }
        }

        public Credential[] ReadCred()
        {
            int Count = 0;
            int Flags = 0x0;
            string Filter = null;
            Credential[] Credentials;

                if (6 <= Environment.OSVersion.Version.Major)
                {
                    Flags = 0x1;
                }
                else
                {
                    string message = "Retrieving all credentials is only possible on Windows version Vista or later.";
                    throw new Exception(message);
                }

            IntPtr pCredentials = IntPtr.Zero;
            bool Read = Imports.CredEnumerate(Filter, Flags, out Count, out pCredentials);
            int LastError = Marshal.GetLastWin32Error();
            
            if (Read)
            {
                CriticalCredentialHandle CredHandle = new CriticalCredentialHandle(pCredentials);
                Credentials = CredHandle.GetCredentials(Count);
                return Credentials;
            }
            else
            {
                string message = string.Format("CredEnumerate failed with the error code {0}.", LastError);
                throw new Exception(message);
            }
        }

    }
}
