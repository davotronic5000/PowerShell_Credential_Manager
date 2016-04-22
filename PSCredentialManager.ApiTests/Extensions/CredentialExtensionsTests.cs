using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSCredentialManager.Api.Extensions;
using PSCredentialManager.Common;
using PSCredentialManager.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PSCredentialManager.Api.Extensions.Tests
{
    [TestClass()]
    public class CredentialExtensionsTests
    {
        [TestMethod()]
        public void ToNativeCredentialTest()
        {
            Credential credential = new Credential()
            {
                AttributeCount = 0,
                Attributes = new IntPtr(0),
                Comment = "This is a comment",
                CredentialBlob = "April123!!",
                CredentialBlobSize = 20,
                Flags = 0,
                LastWritten = new System.Runtime.InteropServices.ComTypes.FILETIME(),
                Persist = CRED_PERSIST.LOCAL_MACHINE,
                TargetName = "server01",
                Type = CRED_TYPE.GENERIC,
                UserName = "test-user"
            };

            NativeCredential nativeCredential = credential.ToNativeCredential();

            Assert.IsNotNull(nativeCredential);
            Assert.IsInstanceOfType(nativeCredential, typeof(NativeCredential));
        }
    }
}