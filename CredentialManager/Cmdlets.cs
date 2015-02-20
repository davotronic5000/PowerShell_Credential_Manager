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
    [Cmdlet(VerbsCommon.Get, "StoredCredential")]
    public class GetStoredCredential : PSCmdlet
    {
        //Parameters
        [Parameter()]
        [ValidateLength(1, 337)]
        public string Target;

        [Parameter()]
        public CRED_TYPE Type = CRED_TYPE.GENERIC;

        [Parameter()]
        public bool AsPsCredential = true;

        //Initiate variables and credential manager
        CredentialManager Manager = new CredentialManager();
        PSCredential PsCredential;

        protected override void BeginProcessing()
        {

        }

        protected override void ProcessRecord()
        {
            if (Target == null)
            {
                //If no target is specified get all available credentials from the store
                Credential[] credential;
                try
                {                    
                    credential = Manager.ReadCred();

                    if (AsPsCredential)
                    {
                        //if AsPSCredential is specified create PS credential object and write to pipeline
                        WriteVerbose("Converting returned credentials to PSCredential Object");
                        foreach (Credential Cred in credential)
                        {
                            try
                            {
                                PsCredential = Cred.ToPsCredential();
                                WriteObject(PsCredential);
                            }
                            catch
                            {
                                WriteWarning("Unable to convert Credential object without username or password to PSCredential object");
                            }
                        }
                    }
                    else
                    {
                        //write credential object to pipeline
                        WriteObject(credential);
                    }
                }
                catch (Exception exception)
                {
                    ErrorRecord errorRecord = new ErrorRecord(exception, "1", ErrorCategory.InvalidOperation, Target);
                    ThrowTerminatingError(errorRecord);
                }
            }
            else
            {
                Credential credential = new Credential();
                try
                {
                    //Retrieve credential from Cred Store
                    WriteVerbose("Retrieving requested credential from Windows Credential Manager");
                    credential = Manager.ReadCred(Target, Type);
                }
                catch (Exception exception)
                {
                    ErrorRecord errorRecord = new ErrorRecord(exception, "1", ErrorCategory.InvalidOperation, Target);
                    ThrowTerminatingError(errorRecord);
                }

                if (AsPsCredential)
                {
                    //if AsPSCredential is specified create PS credential object and write to pipeline
                    WriteVerbose("Converting returned credential blob to PSCredential Object");
                    try
                    {
                        PsCredential = credential.ToPsCredential();
                        WriteObject(PsCredential);
                    }
                    catch
                    {
                        WriteWarning("Unable to convert Credential object without username or password to PSCredential object");
                    }
                }
                else
                {
                    //write credential object to pipeline
                    WriteObject(credential);
                }
            }            
        }

        protected override void EndProcessing()
        {

        }
    }

    [Cmdlet(VerbsCommon.New, "StoredCredential")]
    public class NewStoredCredential : PSCmdlet
    {
        //Parameters
        [Parameter()]
        [ValidateLength(1, 337)]
        public string Target = System.Environment.MachineName;

        [Parameter()]
        public string UserName = System.Environment.UserName.ToString();

        [Parameter()]
        public string Password = System.Web.Security.Membership.GeneratePassword(10, 2);

        [Parameter()]
        public string Comment = "Updated by: " + System.Environment.UserName.ToString() + " on: " + DateTime.Now.ToShortDateString();

        [Parameter()]
        public CRED_TYPE Type = CRED_TYPE.GENERIC;

        [Parameter()]
        public CRED_PERSIST Persist = CRED_PERSIST.SESSION;

        //Initiate variables and credential manager
        CredentialManager Manager = new CredentialManager();
        Credential Credential = new Credential();
        NativeCredential nativeCredential = new NativeCredential();

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
            Credential.Type = Type;
            Credential.Persist = Persist;

            //Convert credential to native credential
            nativeCredential = NativeCredential.ConvertToNativeCredential(Credential);
            
            try
            {
                //Write credential to Windows Credential manager
                WriteVerbose("Writing credential to Windows Credential Manager");
                Manager.WriteCred(nativeCredential);
                WriteObject(Credential);
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

    [Cmdlet(VerbsCommon.Remove, "StoredCredential")]
    public class RemoveStoredCredential : PSCmdlet
    {
        //Parameters
        [Parameter(Mandatory = true)]
        public string Target;

        [Parameter()]
        public CRED_TYPE Type = CRED_TYPE.GENERIC;


        //Initiate Variables
        CredentialManager Manager = new CredentialManager();

        protected override void BeginProcessing()
        {
            
        }

        protected override void ProcessRecord()
        {
            //Delete credential from store
            try
            {
                WriteVerbose("Deleteing requested credential from Windows Credential Manager");
                Manager.DeleteCred(Target, Type);
            }
            catch (Exception exception)
            {
                ErrorRecord errorRecord = new ErrorRecord(exception, "1", ErrorCategory.InvalidOperation, Target);
                WriteError(errorRecord);
            }
        }

        protected override void EndProcessing()
        {
            
        }
    }

    [Cmdlet(VerbsCommon.Get, "StrongPassword")]
    public class GetStrongPassword : PSCmdlet
    {
        //Parameters
        [Parameter()]
        public int Length = 10;

        [Parameter()]
        public int NumberOfSpecialCharacters = 3;

        //Initiate variables
        string Password = string.Empty;

        protected override void BeginProcessing()
        {
            Password = System.Web.Security.Membership.GeneratePassword(Length, NumberOfSpecialCharacters);
            WriteObject(Password);
        }

        protected override void ProcessRecord()
        {
            
        }

        protected override void EndProcessing()
        {
            
        }
    }
}