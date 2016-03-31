using PSCredentialManager.Common;
using System;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;

namespace PSCredentialManager.Cmdlet.Utility
{
    public static class CustomExtensions
    {
        public static SecureString ToSecureString(this string str)
        {
            SecureString secureString = new SecureString();

            foreach (char c in str)
            {
                secureString.AppendChar(c);
            }

            return secureString;
        }

        public static string ToInsecureString(this SecureString secureString)
        {
            IntPtr secureStringPtr = IntPtr.Zero;

            try
            {
                secureStringPtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(secureStringPtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(secureStringPtr);
            }

        }

        public static PSCredential ToPSCredential(this Credential credential)
        {
            PSCredential psCredential;

            try
            {
                if (credential.UserName != null && credential.CredentialBlob != null)
                {
                    SecureString password = new SecureString();
                    foreach (char c in credential.CredentialBlob)
                    {
                        password.AppendChar(c);
                    }
                    psCredential = new PSCredential(credential.UserName, password);
                }
                else
                {
                    throw new Exception("PSCredentialManager.Cmdlet.Utility.PSCredentialUtility.ConvertToPSCredential Unable to convert credential objects with no username");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("PSCredentialManager.Cmdlet.Utility.PSCredentialUtility.ConvertToPSCredential Unable to convert credential object", ex);
            }

            return psCredential;
        }
    }
}
