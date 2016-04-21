using PSCredentialManager.Common;
using PSCredentialManager.Common.Enum;
using System;
using System.Runtime.InteropServices;

namespace PSCredentialManager.Api.Extensions
{
    public static class CredentialExtensions
    {      
        public static NativeCredential ToNativeCredential(this Credential credential)
        {
            NativeCredential nativeCredential;

            try
            {
                nativeCredential = new NativeCredential()
                {
                    AttributeCount = 0,
                    Attributes = IntPtr.Zero,
                    Comment = Marshal.StringToCoTaskMemUni(credential.Comment),
                    TargetAlias = IntPtr.Zero,
                    Type = credential.Type,
                    Persist = (uint)credential.Persist,
                    CredentialBlobSize = (UInt32)credential.CredentialBlobSize,
                    TargetName = Marshal.StringToCoTaskMemUni(credential.TargetName),
                    CredentialBlob = Marshal.StringToCoTaskMemUni(credential.CredentialBlob),
                    UserName = Marshal.StringToCoTaskMemUni(credential.UserName)
                };
            }
            catch (Exception ex)
            {
                throw new Exception("PSCredentialManager.Api.CredentialUtility.ConvertToNativeCredential Unable to convert credential to native credential.", ex);
            }

            return nativeCredential;
        }
    }
}
