using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSCredentialManager.Cmdlet.Extensions;
using PSCredentialManager.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace PSCredentialManager.Cmdlet.Extensions.Tests
{
    [TestClass()]
    public class CredentialExtensionsTests
    {
        [TestMethod()]
        public void ToPSCredentialTest()
        {
            Credential credential = new Credential()
            {
                UserName = "test-user",
                Password = "Password1"
            };

            PSCredential psCredential = credential.ToPSCredential();

            Assert.IsNotNull(psCredential);
            Assert.IsInstanceOfType(psCredential, typeof(PSCredential));
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ToPSCredentialTest1()
        {
            Credential credential = new Credential()
            {
                UserName = "test-user",
            };

            PSCredential psCredential = credential.ToPSCredential();
            
        }
    }
}