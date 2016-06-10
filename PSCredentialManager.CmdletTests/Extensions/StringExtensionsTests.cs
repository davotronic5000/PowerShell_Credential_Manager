using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSCredentialManager.Cmdlet.Extensions;

namespace PSCredentialManager.CmdletTests.Extensions
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