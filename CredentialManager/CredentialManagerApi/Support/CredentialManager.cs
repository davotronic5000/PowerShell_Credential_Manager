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
            int lastError = Marshal.GetLastWin32Error();
            if (written)
            {
                return 0;
            }
            else
            {
                string message = string.Format("CredWrite failed with the error code {0}.", lastError);
                throw new Exception(message);
            }
        }

        public static string ReadCred(string key)
        {
            // Validations.

            IntPtr nCredPtr;
            string readPasswordText = null;

            // Make the API call using the P/Invoke signature
            bool read = Imports.CredRead(key, CRED_TYPE.GENERIC, 0, out nCredPtr);
            int lastError = Marshal.GetLastWin32Error();

            // If the API was successful then...
            if (read)
            {
                using (CriticalCredentialHandle critCred = new CriticalCredentialHandle(nCredPtr))
                {
                    Credential cred = critCred.GetCredential();
                    readPasswordText = cred.CredentialBlob;
                }
            }
            else
            {
                string message = string.Format("ReadCred failed with the error code {0}.", lastError);
                throw new Exception(message);
            }
            return readPasswordText;
        }

        public CRED_ERRORS GetError(uint Error)
        {
            CRED_ERRORS err = new CRED_ERRORS();
            switch (Error)
            {
                case (0x0):
                    err = CRED_ERRORS.ERROR_SUCCESS;
                    break;
                case (0x80070057):
                    err = CRED_ERRORS.ERROR_INVALID_PARAMETER;
                    break;
                case (0x800703EC):
                    err = CRED_ERRORS.ERROR_INVALID_FLAGS;
                    break;
                case (0x80070490):
                    err = CRED_ERRORS.ERROR_NOT_FOUND;
                    break;
                case (0x80070520):
                    err = CRED_ERRORS.ERROR_NO_SUCH_LOGON_SESSION;
                    break;
                case(0x8007089A):
                    err = CRED_ERRORS.ERROR_BAD_USERNAME;
                    break;
                default:
                    err = CRED_ERRORS.ERROR_UNKNOWN_ERROR;
                    break;
            }

            return err;
        }

        public CRED_PERSIST CredPersistFromString(string Persist)
        {
            CRED_PERSIST CredPersist = new CRED_PERSIST();
            switch (Persist)
            {
                case ("SESSION"):
                    CredPersist = CRED_PERSIST.SESSION;
                    break;
                case ("LOCAL_MACHINE"):
                    CredPersist = CRED_PERSIST.LOCAL_MACHINE;
                    break;
                case ("ENTERPRISE"):
                    CredPersist = CRED_PERSIST.ENTERPRISE;
                    break;
                default:
                    CredPersist = CRED_PERSIST.SESSION;
                    break;
            }
            return CredPersist;
        }

        public CRED_TYPE CredTypeFromString(string Type)
        {
            CRED_TYPE CredType = new CRED_TYPE();

            switch (Type)
            {
                case ("GENERIC"):
                    CredType = CRED_TYPE.GENERIC;
                    break;
                case ("DOMAIN_PASSWORD"):
                    CredType = CRED_TYPE.DOMAIN_PASSWORD;
                    break;
                case ("DOMAIN CERTIFICATE"):
                    CredType = CRED_TYPE.DOMAIN_CERTIFICATE;
                    break;
                case ("DOMAIN_VISIBLE_PASSWORD"):
                    CredType = CRED_TYPE.DOMAIN_VISIBLE_PASSWORD;
                    break;
                case ("GENERIC_CERTIFICATE"):
                    CredType = CRED_TYPE.GENERIC_CERTIFICATE;
                    break;
                case ("DOMAIN_EXTENDED"):
                    CredType = CRED_TYPE.DOMAIN_EXTENDED;
                    break;
                case ("MAXIMUM"):
                    CredType = CRED_TYPE.MAXIMUM;
                    break;
                case ("MAXIMUM_EX"):
                    CredType = CRED_TYPE.MAXIMUM_EX;
                    break;
                default:
                    CredType = CRED_TYPE.GENERIC;
                    break;
            }
            return CredType;
        }
    }
}
