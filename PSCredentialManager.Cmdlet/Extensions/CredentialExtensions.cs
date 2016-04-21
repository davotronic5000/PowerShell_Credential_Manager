using PSCredentialManager.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security;
using System.Text;

namespace PSCredentialManager.Cmdlet.Extensions
{
    public static class CredentialExtensions
    {
        public static PSCredential ToPSCredential(this Credential credential)
        {
            PSCredential psCredential;

            try
            {
                if (credential.UserName != null && credential.CredentialBlob != null)
                {
                    SecureString password = new SecureString();
                    foreach (char c in credential.CredentialBlob)
                    {
                        password.AppendChar(c);
                    }
                    psCredential = new PSCredential(credential.UserName, password);
                }
                else
                {
                    throw new Exception("PSCredentialManager.Cmdlet.Utility.PSCredentialUtility.ConvertToPSCredential Unable to convert credential objects with no username");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("PSCredentialManager.Cmdlet.Utility.PSCredentialUtility.ConvertToPSCredential Unable to convert credential object", ex);
            }

            return psCredential;
        }
    }
}
