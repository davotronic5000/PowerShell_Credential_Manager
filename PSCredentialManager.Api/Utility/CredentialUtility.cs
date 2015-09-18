using PSCredentialManager.Common;
using PSCredentialManager.Object.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PSCredentialManager.Api.Utility
{
    public static class CredentialUtility
    {
        public static Credential ConvertToCredential(NativeCredential nativeCredential)
        {
            Credential credential;

            try
            {
                credential = new Credential()
                {
                    Type = nativeCredential.Type,
                    Flags = nativeCredential.Flags,
                    Persist = (CRED_PERSIST)nativeCredential.Persist,
                    UserName = Marshal.PtrToStringUni(nativeCredential.UserName),
                    TargetName = Marshal.PtrToStringUni(nativeCredential.TargetName),
                    TargetAlias = Marshal.PtrToStringUni(nativeCredential.TargetAlias),
                    Comment = Marshal.PtrToStringUni(nativeCredential.Comment),
                    CredentialBlobSize = nativeCredential.CredentialBlobSize
                };

                if (0 < nativeCredential.CredentialBlobSize)
                {
                    credential.CredentialBlob = Marshal.PtrToStringUni(nativeCredential.CredentialBlob, (int)nativeCredential.CredentialBlobSize / 2);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("PSCredentialManager.Api.CredentialUtility.ConvertToCredential Unable to convert native credential to credential.", ex);
            }

            return credential;
        }

        public static NativeCredential ConvertToNativeCredential(Credential credential)
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
