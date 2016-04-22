using PSCredentialManager.Api.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using PSCredentialManager.Common;
using PSCredentialManager.Common.Enum;

namespace PSCredentialManager.Api.Extensions.Tests
{
    [TestClass()]
    public class NativeCredentialExtensionsTests
    {
        [TestMethod()]
        public void ToCredentialTest()
        {
            NativeCredential nativeCredential = new NativeCredential()
            {
                AttributeCount = 0,
                Attributes = new IntPtr(0),
                Comment = new IntPtr(0),
                CredentialBlob = new IntPtr(0),
                CredentialBlobSize = 0,
                Flags = 0,
                LastWritten = new System.Runtime.InteropServices.ComTypes.FILETIME(),
                Persist = 0,
                TargetAlias = new IntPtr(0),
                TargetName = new IntPtr(0),
                Type = Cred_Type.GENERIC,
                UserName = new IntPtr(0)
            };

            Credential credential = nativeCredential.ToCredential();

            Assert.IsNotNull(credential);
            Assert.IsInstanceOfType(credential, typeof(Credential));
        }
    }
}