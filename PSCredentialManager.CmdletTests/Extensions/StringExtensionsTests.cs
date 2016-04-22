using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSCredentialManager.Cmdlet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace PSCredentialManager.Cmdlet.Extensions.Tests
{
    [TestClass()]
    public class StringExtensionsTests
    {
        [TestMethod()]
        public void ToSecureStringTest()
        {
            SecureString secureString = "InsecureString".ToSecureString();

            Assert.IsNotNull(secureString);
            Assert.IsInstanceOfType(secureString, typeof(SecureString));
        }
    }
}