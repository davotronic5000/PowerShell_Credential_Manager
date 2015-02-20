using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSCredentialManager.CredentialManagerApi.Enums;
using System.Runtime.InteropServices;
using System.Management.Automation;
using System.Security;

namespace PSCredentialManager.CredentialManagerApi.DataStructures
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct Credential
    {
        public UInt32 Flags;
        public CRED_TYPE Type;
        public string TargetName;
        public string Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public UInt32 CredentialBlobSize;
        public string CredentialBlob;
        public CRED_PERSIST Persist;
        public UInt32 AttributeCount;
        public IntPtr Attributes;
        public string TargetAlias;
        public string UserName;
    }
}
