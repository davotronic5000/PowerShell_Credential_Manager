using PSCredentialManager.Common;
using System;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;

namespace PSCredentialManager.Cmdlet.Extensions
{
    public static class StringExtensions
    {
        public static SecureString ToSecureString(this string insecureString)
        {
            SecureString secureString = new SecureString();

            foreach (char character in insecureString)
            {
                secureString.AppendChar(character);
            }

            return secureString;
        }
    }
}
