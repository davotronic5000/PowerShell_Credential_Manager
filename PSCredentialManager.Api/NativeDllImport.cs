using PSCredentialManager.Common;
using PSCredentialManager.Common.Enum;
using System;
using System.Runtime.InteropServices;

namespace PSCredentialManager.Api
{
    public class Imports
    {
        public static bool CredRead(string target, CRED_TYPE type, int reservedFlag, out IntPtr credentialPointer)
        {
            return NativeMethods.CredRead(target, type, reservedFlag, out credentialPointer);
        }

        public static bool CredWrite(ref NativeCredential userCredential, UInt32 flags)
        {
            return NativeMethods.CredWrite(ref userCredential, flags);
        }
        
        public static bool CredFree(IntPtr credentialPointer)
        {
            return NativeMethods.CredFree(credentialPointer);
        }

        public static bool CredDelete(string target, CRED_TYPE type, int reservedFlag)
        {
            return NativeMethods.CredDelete(target, type, reservedFlag);
        }

        public static bool CredEnumerate(string filter, int flags, out int count, out IntPtr credentialPointers)
        {
            return NativeMethods.CredEnumerate(filter, flags, out count, out credentialPointers);
        }



        private class NativeMethods
        {
            [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool CredRead([In] string target, [In] CRED_TYPE type, [In] int reservedFlag, out IntPtr credentialPtr);

            [DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool CredWrite([In] ref NativeCredential userCredential, [In] UInt32 flags);

            [DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
            public static extern bool CredFree([In] IntPtr credentialPointer);

            [DllImport("Advapi32.dll", SetLastError = true, EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode)]
            public static extern bool CredDelete([In] string target, [In] CRED_TYPE type, [In] int reservedFlag);

            [DllImport("Advapi32.dll", SetLastError = true, EntryPoint = "CredEnumerateW", CharSet = CharSet.Unicode)]
            public static extern bool CredEnumerate([In] string filter, [In] int flags, out int count, out IntPtr credentialPtrs);
        }
        
    }
}
