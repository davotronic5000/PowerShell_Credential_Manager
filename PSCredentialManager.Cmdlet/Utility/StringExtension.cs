using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace PSCredentialManager.Utility
{
    public static class StringExtension
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
