using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using PSCredentialManager.CredentialManagerApi;
using System.Runtime.InteropServices;
using PSCredentialManager.CredentialManagerApi.Enums;
using PSCredentialManager.CredentialManagerApi.DataStructures;
using PSCredentialManager.CredentialManagerApi.Support;
using System.Security;

namespace PSCredentialManager
{
    //[Cmdlet(VerbsCommon.Get, "StoredCredential")]
    //public class GetCredential : PSCmdlet
    //{
    //    //Parameters
    //    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    //    [ValidateLength(1, 337)]
    //    public string Target;

    //    [Parameter()]
    //    [ValidateSet("GENERIC", "DOMAIN_PASSWORD", "DOMAIN_CERTIFICATE", "DOMAIN_VISIBLE_PASSWORD", "GENERIC_CERTIFICATE", "DOMAIN_EXTENDED", "MAXIMUM", "MAXIMUM_EX")]
    //    public string Type = "GENERIC";

    //    [Parameter()]
    //    public bool AsPsCredential = true;

    //    //Initiate variables and credential manager
    //    CRED_TYPE CredType = new CRED_TYPE();
    //    CredentialManager Manager = new CredentialManager();
    //    PSCredential PsCred;

    //    protected override void BeginProcessing()
    //    {
    //        try
    //        {
    //            CredType = Manager.CredTypeFromString(Type);
    //        }
    //        catch
    //        {
    //            //write error here
    //        }
    //    }

    //    protected override void ProcessRecord()
    //    {
    //        Credential Cred = new Credential();

    //        try
    //        {
    //            WriteVerbose("Retrieving Credential record from Windows Credential Manager");
    //            Manager.CredRead(Target, CredType, out Cred);
    //        }
    //        catch (Exception Ex)
    //        {
    //            //Write Error
    //        }

    //        if (Cred.TargetName != null)
    //        {
    //            if (AsPsCredential)
    //            {
    //                try
    //                {
    //                    //Create PSCredential Object
    //                    WriteVerbose("Converting returned credential blob to PSCredential Object");
    //                    PsCred = Cred.ToPsCredential();
    //                }
    //                catch (Exception Ex)
    //                {
    //                    //Write Error here
    //                }

    //                WriteObject(PsCred);
    //            }
    //            else
    //            {
    //                WriteObject(Cred);
    //            }
    //        }
    //    }

    //    protected override void EndProcessing()
    //    {

    //    }
    //}

    [Cmdlet(VerbsCommon.New, "StoredCredential")]
    public class NewCredential : PSCmdlet
    {
        //Parameters
        [Parameter(Mandatory = true)]
        [ValidateLength(1, 337)]
        public string Target;

        [Parameter()]
        public string UserName = System.Environment.UserName.ToString();

        [Parameter(Mandatory = true)]
        public string Password;

        [Parameter()]
        public string Comment = "Updated by: " + System.Environment.UserName.ToString() + "on: " + DateTime.Now.ToShortDateString();

        [Parameter()]
        [ValidateSet("GENERIC", "DOMAIN_PASSWORD", "DOMAIN_CERTIFICATE", "DOMAIN_VISIBLE_PASSWORD", "GENERIC_CERTIFICATE", "DOMAIN_EXTENDED", "MAXIMUM", "MAXIMUM_EX")]
        public string Type = "GENERIC";

        [Parameter()]
        [ValidateSet("SESSION", "LOCAL_MACHINE", "ENTERPRISE")]
        public string Persist = "SESSION";

        //Initiate variables and credential manager
        CredentialManager Manager = new CredentialManager();
        Credential Credential = new Credential();
        NativeCredential nativeCredential = new NativeCredential();
        CRED_PERSIST CredPersist = new CRED_PERSIST();
        CRED_TYPE CredType = new CRED_TYPE();

        protected override void BeginProcessing()
        {
            //Argument validation
            //Check password does not exceed 512 bytes
            byte[] byteArray = Encoding.Unicode.GetBytes(Password);
            if (byteArray.Length > 512)
            {                
                Exception exception = new ArgumentOutOfRangeException("Password", "The specified password has exceeded 512 bytes");
                ErrorRecord error = new ErrorRecord(exception, "1", ErrorCategory.InvalidArgument , Password);
                WriteError(error);
            }

            //Convert Type and Persistance to marshalable properties
            CredPersist = Manager.CredPersistFromString(Persist);
            CredType = Manager.CredTypeFromString(Type);
                
        }

        protected override void ProcessRecord()
        {
            //Create credential object
            Credential.TargetName = Target;
            Credential.CredentialBlob = Password;
            Credential.CredentialBlobSize = (UInt32)Encoding.Unicode.GetBytes(Password).Length;
            Credential.AttributeCount = 0;
            Credential.Attributes = IntPtr.Zero;
            Credential.Comment = Comment;
            Credential.TargetAlias = null;
            Credential.Type = CredType;
            Credential.Persist = CredPersist;

            //Convert credential to native credential
            nativeCredential = NativeCredential.GetNativeCredential(Credential);
            
            try
            {
                //Write credential to Windows Credential manager
                WriteVerbose("Writing credential to Windows Credential Manager");
                Manager.WriteCred(nativeCredential);
            }
            catch (Exception exception)
            {
                ErrorRecord errorRecord = new ErrorRecord(exception, "1", ErrorCategory.InvalidOperation, Credential);
                WriteError(errorRecord);
            }
        }

        protected override void EndProcessing()
        {

        }
    }
}