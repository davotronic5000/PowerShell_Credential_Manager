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
    public class PsCredentialCmdlets
    {
        static readonly CredentialManager CredentialManager = new CredentialManager();

        [Cmdlet(VerbsCommon.Get, "StoredCredential")]
        [OutputType(typeof(PSCredential))]
        [OutputType(typeof(Credential), ParameterSetName = new[]{"CredentialObject Output"})]
        public class GetStoredCredential : PSCmdlet
        {
            //Parameters
            [Parameter(ValueFromPipeline = true)]
            [ValidateLength(1, 337)]
            public string Target;

            [Parameter()]
            public CredType Type = CredType.Generic;

            [Parameter(ParameterSetName = "CredentialObject Output")]
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
                        credential = CredentialManager.ReadCred();

                        if (!AsCredentialObject)
                        {                     

                            //if AsPSCredential is specified create PS credential object and write to pipeline
                            WriteVerbose("Converting returned credentials to PSCredential Object");
                            
                            foreach (Credential cred in credential)
                            {

                                try
                                {
                                    PSCredential psCredential = cred.ToPsCredential();
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
                        credential = CredentialManager.ReadCred(Target, Type);
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
                                PSCredential psCredential = credential.ToPsCredential();
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
        [OutputType(typeof(Credential))]
        public class NewStoredCredential : PSCmdlet
        {
            //Parameters
            [Parameter()]
            [ValidateLength(1, 337)]
            public string Target = Environment.MachineName;

            [Parameter(ParameterSetName = "Plain Text")]
            [Parameter(ParameterSetName = "Secure String")]
            public string UserName = Environment.UserName;

            [Parameter(ParameterSetName = "Plain Text")]
            public string Password = System.Web.Security.Membership.GeneratePassword(10, 2);

            [Parameter(ParameterSetName = "Secure String")]
            public SecureString SecurePassword;

            [Parameter()]
            public string Comment = "Updated by: " + Environment.UserName + " on: " + DateTime.Now.ToShortDateString();

            [Parameter()]
            public CredType Type = CredType.Generic;

            [Parameter()]
            public CredPersist Persist = CredPersist.Session;

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
                        Exception exception = new ArgumentOutOfRangeException($"Password", "The specified password has exceeded 512 bytes");
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
                        Exception exception = new ArgumentOutOfRangeException($"SecurePassword", "The specified password has exceeded 512 bytes");
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
                    Password = Password,
                    PaswordSize = (UInt32)Encoding.Unicode.GetBytes(Password).Length,
                    AttributeCount = 0,
                    Attributes = IntPtr.Zero,
                    Comment = Comment,
                    TargetAlias = null,
                    Type = Type,
                    Persist = Persist,
                    UserName = UserName,
                    LastWritten = DateTime.Now,                    
                };
            
                

                //Convert credential to native credential
                NativeCredential nativeCredential = credential.ToNativeCredential();

                try
                {
                    //Write credential to Windows Credential manager
                    WriteVerbose("Writing credential to Windows Credential Manager");
                    CredentialManager.WriteCred(nativeCredential);
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
            public CredType Type = CredType.Generic;

            protected override void BeginProcessing() { }

            protected override void ProcessRecord()
            {
                //Delete credential from store
                try
                {
                    WriteVerbose("Deleting requested credential from Windows Credential Manager");
                    CredentialManager.DeleteCred(Target, Type);
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
        [OutputType(typeof(string))]
        public class GetStrongPassword : PSCmdlet
        {
            //Parameters
            [Parameter()]
            public int Length = 10;

            [Parameter()]
            public int NumberOfSpecialCharacters = 3;

            protected override void BeginProcessing() { }

            protected override void ProcessRecord()
            {
                WriteObject(System.Web.Security.Membership.GeneratePassword(Length, NumberOfSpecialCharacters));
            }

            protected override void EndProcessing() { }
        }
    }
}