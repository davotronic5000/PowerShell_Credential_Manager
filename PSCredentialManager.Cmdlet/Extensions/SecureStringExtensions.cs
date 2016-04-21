using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace PSCredentialManager.Cmdlet.Extensions
{
    public static class SecureStringExtensions
    {
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
    }
}
