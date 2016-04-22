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
                Password = "April123!!",
                PaswordSize = 20,
                Flags = 0,
                LastWritten = DateTime.Now,
                Persist = Cred_Persist.LOCAL_MACHINE,
                TargetName = "server01",
                Type = Cred_Type.GENERIC,
                UserName = "test-user"
            };

            NativeCredential nativeCredential = credential.ToNativeCredential();

            Assert.IsNotNull(nativeCredential);
            Assert.IsInstanceOfType(nativeCredential, typeof(NativeCredential));
        }

    }
}