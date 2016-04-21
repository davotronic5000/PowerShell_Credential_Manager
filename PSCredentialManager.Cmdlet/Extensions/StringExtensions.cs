using PSCredentialManager.Common;
using System;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;

namespace PSCredentialManager.Cmdlet.Extensions
{
    public static class StringExtensions
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
    }
}
