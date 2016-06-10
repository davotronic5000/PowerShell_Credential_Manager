using System.Linq;
using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSCredentialManager.Cmdlet.Extensions;

namespace PSCredentialManager.CmdletTests.Extensions
{
    [TestClass()]
    public class SecureStringExtensionsTests
    {
        [TestMethod()]
        public void ToInsecureStringTest()
        {
            SecureString secureString = new SecureString();
            "SecureString".ToCharArray().ToList().ForEach(p => secureString.AppendChar(p));

            string insecureString = secureString.ToInsecureString();

            Assert.IsNotNull(insecureString);
            Assert.IsInstanceOfType(insecureString, typeof(string));
            Assert.AreEqual(insecureString, "SecureString");
        }
    }
}