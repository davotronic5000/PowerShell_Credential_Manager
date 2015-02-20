using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSCredentialManager.CredentialManagerApi.Enums;
using System.Runtime.InteropServices;

namespace PSCredentialManager.CredentialManagerApi.DataStructures
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
     struct NativeCredential
    {
        public UInt32 Flags;
        public CRED_TYPE Type;
        public IntPtr TargetName;
        public IntPtr Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public UInt32 CredentialBlobSize;
        public IntPtr CredentialBlob;
        public UInt32 Persist;
        public UInt32 AttributeCount;
        public IntPtr Attributes;
        public IntPtr TargetAlias;
        public IntPtr UserName;
      
        internal static NativeCredential GetNativeCredential(Credential Credential)
        {
            NativeCredential nativeCredential = new NativeCredential();
            nativeCredential.AttributeCount = 0;
            nativeCredential.Attributes = IntPtr.Zero;
            nativeCredential.Comment = Marshal.StringToCoTaskMemUni(Credential.Comment);
            nativeCredential.TargetAlias = IntPtr.Zero;
            nativeCredential.Type = Credential.Type;
            nativeCredential.Persist = (uint)Credential.Persist;
            nativeCredential.CredentialBlobSize = (UInt32)Credential.CredentialBlobSize;
            nativeCredential.TargetName = Marshal.StringToCoTaskMemUni(Credential.TargetName);
            nativeCredential.CredentialBlob = Marshal.StringToCoTaskMemUni(Credential.CredentialBlob);
            nativeCredential.UserName = Marshal.StringToCoTaskMemUni(System.Environment.UserName);
            return nativeCredential;
        }
    }
}
