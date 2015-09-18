using System;
using PSCredentialManager.CredentialManagerApi.Enums;
using System.Runtime.InteropServices;
using System.Management.Automation;
using System.Security;

namespace PSCredentialManager.CredentialManagerApi.DataStructures
{

        public PSCredential ToPsCredential()
        {
            if (this.UserName != null && this.CredentialBlob != null)
            {
                SecureString Password = new SecureString();
                PSCredential PsCred;
                foreach (char c in this.CredentialBlob)
                {
                    Password.AppendChar(c);
                }
                PsCred = new PSCredential(this.UserName, Password);
                return PsCred;
            }
            else
            {
                throw new Exception("Unable to convert credential with now username or password to PS Credential");
            }
            
        }
    }
}
