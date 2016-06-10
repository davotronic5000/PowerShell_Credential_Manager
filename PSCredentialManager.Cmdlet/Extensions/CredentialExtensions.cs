using PSCredentialManager.Common;
using System;
using System.Management.Automation;

namespace PSCredentialManager.Cmdlet.Extensions
{
    public static class CredentialExtensions
    {
        public static PSCredential ToPsCredential(this Credential credential)
        {
            PSCredential psCredential;

            try
            {
                if (credential.UserName != null && credential.Password != null)
                {                                 
                    psCredential = new PSCredential(credential.UserName, credential.Password.ToSecureString());
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
