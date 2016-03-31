using PSCredentialManager.Common.Enum;
using System;
using System.Runtime.InteropServices;

namespace PSCredentialManager.Common
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct Credential
    {
        public uint Flags;
        public CRED_TYPE Type;
        public string TargetName;
        public string Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public uint CredentialBlobSize;
        public string CredentialBlob;
        public CRED_PERSIST Persist;
        public uint AttributeCount;
        public IntPtr Attributes;
        public string TargetAlias;
        public string UserName;
    }
}
