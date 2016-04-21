using System;
using System.Text;
using System.Management.Automation;
using PSCredentialManager.Common.Enum;
using PSCredentialManager.Api;
using PSCredentialManager.Common;
using PSCredentialManager.Api.Extensions;
using PSCredentialManager.Common.Exceptions;
using System.Collections.Generic;
using System.Security;
using PSCredentialManager.Cmdlet.Extensions;

namespace PSCredentialManager.Cmdlet
{
    public class PSCredentialCmdlets
    {
        internal static CredentialManager credentialManager = new CredentialManager();

        [Cmdlet(VerbsCommon.Get, "StoredCredential")]
        public class GetStoredCredential : PSCmdlet
        {
            //Parameters
            [Parameter(ValueFromPipeline = true)]
            [ValidateLength(1, 337)]
            public string Target;

            [Parameter()]
            public CRED_TYPE Type = CRED_TYPE.GENERIC;

            [Parameter()]
            public SwitchParameter AsCredentialObject;

            //Initiate variables and credential manager



            protected override void BeginProcessing() { }

            protected override void ProcessRecord()
            {               

                if (Target == null)
                {
                    //If no target is specified get all available credentials from the store
                    IEnumerable<Credential> credential;
                    try
                    {
                        credential = credentialManager.ReadCred();

                        if (!AsCredentialObject)
                        {                     

                            //if AsPSCredential is specified create PS credential object and write to pipeline
                            WriteVerbose("Converting returned credentials to PSCredential Object");
                            
                            foreach (Credential cred in credential)
                            {

                                try
                                {
                                    PSCredential psCredential = cred.ToPSCredential();
                                    WriteObject(psCredential);
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
                        WriteError(errorRecord);
                    }
                }
                else
                {
                    Credential credential = new Credential();
                    bool shouldContinue = true;
                    try
                    {
                        //Retrieve credential from Cred Store
                        WriteVerbose("Retrieving requested credential from Windows Credential Manager");
                        credential = credentialManager.ReadCred(Target, Type);
                    }
                    catch (CredentialNotFoundException exception)
                    {
                        WriteDebug(exception.Message);
                        shouldContinue = false;
                    }
                    catch (Exception exception)
                    {
                        ErrorRecord errorRecord = new ErrorRecord(exception, "1", ErrorCategory.InvalidOperation, Target);
                        WriteError(errorRecord);
                    }

                    if (shouldContinue)
                    {
                        if (!AsCredentialObject)
                        {
                            //if AsPSCredential is specified create PS credential object and write to pipeline
                            WriteVerbose("Converting returned credential blob to PSCredential Object");
                            try
                            {
                                PSCredential psCredential = credential.ToPSCredential();
                                WriteObject(psCredential);
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
            }

            protected override void EndProcessing() { }
        }

        [Cmdlet(VerbsCommon.New, "StoredCredential")]
        public class NewStoredCredential : PSCmdlet
        {
            //Parameters
            [Parameter()]
            [ValidateLength(1, 337)]
            public string Target = System.Environment.MachineName;

            [Parameter(ParameterSetName = "Plain Text")]
            [Parameter(ParameterSetName = "Secure String")]
            public string UserName = System.Environment.UserName.ToString();

            [Parameter(ParameterSetName = "Plain Text")]
            public string Password = System.Web.Security.Membership.GeneratePassword(10, 2);

            [Parameter(ParameterSetName = "Secure String")]
            public SecureString SecurePassword;

            [Parameter()]
            public string Comment = "Updated by: " + System.Environment.UserName.ToString() + " on: " + DateTime.Now.ToShortDateString();

            [Parameter()]
            public CRED_TYPE Type = CRED_TYPE.GENERIC;

            [Parameter()]
            public CRED_PERSIST Persist = CRED_PERSIST.SESSION;

            [Parameter(ValueFromPipeline = true, ParameterSetName = "PSCredentialObject")]
            public PSCredential Credentials;

            protected override void BeginProcessing()
            {
                //Check if Password or SecurePassword is set and validate the password is valid.

                if (MyInvocation.BoundParameters.ContainsKey("Password"))
                {
                    //Argument validation
                    //Check password does not exceed 512 bytes
                    byte[] byteArray = Encoding.Unicode.GetBytes(Password);
                    if (byteArray.Length > 512)
                    {
                        Exception exception = new ArgumentOutOfRangeException("Password", "The specified password has exceeded 512 bytes");
                        ErrorRecord error = new ErrorRecord(exception, "1", ErrorCategory.InvalidArgument, Password);
                        WriteError(error);
                    }
                }

                if (MyInvocation.BoundParameters.ContainsKey("SecurePassword"))
                {
                    //Argument validation
                    //Check password does not exceed 512 bytes
                    byte[] byteArray = Encoding.Unicode.GetBytes(Password);
                    if (byteArray.Length > 512)
                    {
                        Exception exception = new ArgumentOutOfRangeException("SecurePassword", "The specified password has exceeded 512 bytes");
                        ErrorRecord error = new ErrorRecord(exception, "1", ErrorCategory.InvalidArgument, Password);
                        WriteError(error);
                    }
                    Password = SecurePassword.ToInsecureString();
                }
            }

            protected override void ProcessRecord()
            {
                if (MyInvocation.BoundParameters.ContainsKey("Credentials"))
                {
                    UserName = Credentials.UserName;
                    Password = Credentials.GetNetworkCredential().Password;
                }
                
                //Create credential object
                Credential credential = new Credential()
                {
                    TargetName = Target,
                    CredentialBlob = Password,
                    CredentialBlobSize = (UInt32)Encoding.Unicode.GetBytes(Password).Length,
                    AttributeCount = 0,
                    Attributes = IntPtr.Zero,
                    Comment = Comment,
                    TargetAlias = null,
                    Type = Type,
                    Persist = Persist,
                    UserName = UserName
                };
            
                

                //Convert credential to native credential
                NativeCredential nativeCredential = credential.ToNativeCredential();

                try
                {
                    //Write credential to Windows Credential manager
                    WriteVerbose("Writing credential to Windows Credential Manager");
                    credentialManager.WriteCred(nativeCredential);
                    WriteObject(credential);
                }
                catch (Exception exception)
                {
                    ErrorRecord errorRecord = new ErrorRecord(exception, "1", ErrorCategory.InvalidOperation, credential);
                    WriteError(errorRecord);
                }
            }

            protected override void EndProcessing() { }
        }

        [Cmdlet(VerbsCommon.Remove, "StoredCredential")]
        public class RemoveStoredCredential : PSCmdlet
        {
            //Parameters
            [Parameter(Mandatory = true, ValueFromPipeline = true)]
            public string Target;

            [Parameter()]
            public CRED_TYPE Type = CRED_TYPE.GENERIC;

            protected override void BeginProcessing() { }

            protected override void ProcessRecord()
            {
                //Delete credential from store
                try
                {
                    WriteVerbose("Deleting requested credential from Windows Credential Manager");
                    credentialManager.DeleteCred(Target, Type);
                }
                catch (Exception exception)
                {
                    ErrorRecord errorRecord = new ErrorRecord(exception, "1", ErrorCategory.InvalidOperation, Target);
                    WriteError(errorRecord);
                }
            }

            protected override void EndProcessing() { }
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

            protected override void BeginProcessing() { }

            protected override void ProcessRecord()
            {
                WriteObject(System.Web.Security.Membership.GeneratePassword(Length, NumberOfSpecialCharacters));
            }

            protected override void EndProcessing() { }
        }
    }
}